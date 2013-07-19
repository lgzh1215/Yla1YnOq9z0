using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Serialization;
using Ionic.Zip;
using Common.Logging;
using SSWSyncer.Launcher.AppUpdater;

namespace SSWSyncer.Launcher.AppUpdater.Wpf {
    public class Updater {
        private static ILog log = LogManager.GetLogger(typeof(Updater));
        private Object _lock = new Object();
        private bool cancelAction;
        private String _manifestUrl;
        private Assembly _a;
        private String _infoUrl;
        private String _publicKeyXml;
        private Nullable<bool> _wantUpdate;
        private Exception _LastException;
        private String _manifestXml;
        private List<Assembly> _assemblies;
        //private List<FileInfo> _dbxmls;
        private UpdateManifest _updateManifest;
        private String _mutexId;
        Window window;
        ProgressBar pbDownload = null;
        private Dictionary<String, String> CommandLineArgs;
        private SimpleUpdateInfo _info = null;
        //private String uncAV = "uncAV.xml";
        int totalBytesTransferred = 0;

        private Updater () {
        }

        public Updater (string infoUrl, string manifestUrl, string pubKeyXml) {
            log.Debug("_CurrentExeName:" + _CurrentExeName);
            _manifestUrl = manifestUrl;
            _infoUrl = infoUrl;
            _publicKeyXml = pubKeyXml;
            BuildArgDictionary();

            byte[] hash = ComputeHash(_CurrentExeName);
            _mutexId = ByteArrayToString(hash);
            System.Threading.Mutex _mutex = new System.Threading.Mutex(false, _mutexId);
            log.Debug("my mutexId:        " + _mutexId);
            // create the mutex, and acquire it.
            while (!_mutex.WaitOne(60, false))
                ;
            log.Debug("got startup mutex: " + _mutexId);
            // The mutex will be released when the process exits. 
            // The app uses this mutex to signal when a process exits, so that
            // the exe can be deleted or overwritten.

            if (CommandLineArgs.ContainsValue("phase2")) {
                Phase2_CopyAndDie();
            } else if (CommandLineArgs.ContainsValue("phase3")) {
                Phase3_MaybeCleanUp();
            } else {
                CheckLatest();
            }
        }

        #region Public Properties
        public String PublicKeyXml {
            get {
                return _publicKeyXml;
            }
            set {
                _publicKeyXml = value;
            }
        }

        public Exception LastException {
            get {
                return _LastException;
            }
        }

        public bool UpdateIsAvailableAndValid {
            get {
                if (_updateManifest == null) {
                    GetUpdateInfo();
                }
                return _updateManifest.UpdateIsAvailableAndValid;
            }
        }

        public Dictionary<String, String> UpdaterArgs {
            get {
                return CommandLineArgs;
            }
        }
        #endregion

        #region public methods
        public bool IsUpdateAvailable () {
            lock (_lock) {
                if (_wantUpdate.HasValue) {
                    return _wantUpdate.Value;
                }
                GetUpdateInfo();
                bool wantUpdate = false;
                List<Assembly> assemblies = new List<Assembly>();
                foreach (SimpleUpdateInfo info in _updateManifest.Manifest) {
                    var assemblyFullName = info.AssemblyFullName.ToString();
                    var assemblyName = assemblyFullName.Substring(0, assemblyFullName.IndexOf(','));
                    try {
                        Assembly asm = Assembly.Load(assemblyName);
                        if (info.ImageName == asm.ManifestModule.Name) {
                            log.Debug("Manifest assembly:" + asm.GetName().Name);
                            assemblies.Add(asm);
                        }
                    } catch {
                    }
                }
                _assemblies = new List<Assembly>();
                List<Assembly> forceUpdateAsm = new List<Assembly>();
                foreach (Assembly asm in assemblies) {
                    wantUpdate = false;
                    string CurrentVersion = asm.GetName().Version.ToString();
                    var CurrentVersionInts = CurrentVersion.Split(".".ToCharArray()).ToList<String>().
                            ConvertAll(s => Convert.ToInt32(s));
                    foreach (SimpleUpdateInfo info in _updateManifest.Manifest) {
                        if (info.ImageName == asm.ManifestModule.Name) {
                            if (info.ForceUpdate) {
                                log.Debug("Update Available:" + info.AssemblyFullName);
                                forceUpdateAsm.Add(asm);
                            } else {
                                var LatestVersionInts = info.LatestAvailableVersion.Split(".".ToCharArray()).
                                    ToList<String>().ConvertAll(s => Convert.ToInt32(s));
                                for (int i = 0; i < CurrentVersionInts.Count; i++) {
                                    if (i < LatestVersionInts.Count) {
                                        if (CurrentVersionInts[i] < LatestVersionInts[i]) {
                                            log.Debug("Update Available:" + info.AssemblyFullName);
                                            _assemblies.Add(asm);
                                            wantUpdate = true;
                                        } else if (CurrentVersionInts[i] > LatestVersionInts[i]) {
                                            break;
                                        }
                                    }
                                    if (wantUpdate)
                                        break;
                                }
                                if (wantUpdate)
                                    break;
                            }
                        }
                    }
                }
                if (_assemblies.Count > 0) {
                    wantUpdate = true;
                    _updateManifest.UpdateIsAvailableAndValid = true;
                    _assemblies.AddRange(forceUpdateAsm);
                }
                _wantUpdate = new Nullable<bool>(wantUpdate);
            }
            return _wantUpdate.Value;
        }

        #endregion

        #region private methods

        private void CheckLatest () {
            MessageListener.Instance.ReceiveMessage("Check update info");
            var isUpdate = IsUpdateAvailable();
            if (isUpdate) {
                Phase1_DownloadUpdateAndDie();
            }
        }

        private void Phase1_DownloadUpdateAndDie () {
            MessageListener.Instance.ReceiveMessage("get update files...");
            String tempDir = System.Environment.GetEnvironmentVariable("TEMP");

            String tempBase = Path.Combine(tempDir, Path.GetRandomFileName());
            while (Directory.Exists(tempBase)) {
                tempBase = Path.GetTempPath() + Path.GetRandomFileName();
            }
            File.Delete(tempBase);
            Directory.CreateDirectory(tempBase);
            log.Debug("phase1: download to origBase: " + tempBase);
            try {
                List<String> imageNames = new List<String>();
                List<String> urls = new List<String>();
                foreach (Assembly asm in _assemblies) {
                    String url = _updateManifest.CodeBase + "/" + asm.ManifestModule.Name;
                    log.Debug("url:" + url);
                    imageNames.Add(Path.Combine(tempBase, asm.ManifestModule.Name));
                    urls.Add(url);
                }
                StartDownloadImages(tempBase, imageNames.ToArray(), urls.ToArray());
            } catch (Exception ex1) {
                _LastException = ex1;
            }
        }

        private void StartDownloadImages (String tempBase, String[] imageNames, String[] urls) {
            window = Application.Current.MainWindow;
            log.Debug("StartDownloadImages to " + tempBase);

            pbDownload = LogicalTreeHelper.FindLogicalNode(window, "pbDownload") as ProgressBar;
            int imageSize = 0;
            List<int> contentLengths = new List<int>();
            foreach (String file in urls) {
                int size = GetContentLength(file);
                imageSize += size;
                contentLengths.Add(size);
            }
            pbDownload.Maximum = imageSize;
            pbDownload.Visibility = Visibility.Visible;
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            Object[] args = { tempBase, urls, imageNames, contentLengths.ToArray() };

            var downloader = new System.ComponentModel.BackgroundWorker();
            downloader.DoWork += DownloadWorker;
            downloader.RunWorkerCompleted += DownloadCompleted;
            downloader.RunWorkerAsync(args);
            window.Show();
        }

        // need the length for the progress bar
        private int GetContentLength (string url) {
            System.Net.WebHeaderCollection headers = null;
            System.Net.HttpWebResponse response = null;
            try {
                System.Net.HttpWebRequest request = System.Net.WebRequest.Create(url) as System.Net.HttpWebRequest;
                MarkNoCache(request);
                request.Method = "HEAD";
                response = request.GetResponse() as System.Net.HttpWebResponse;
                headers = response.Headers;
            } finally {
                //  avoid leaking connections
                if (response != null)
                    response.Close();
            }
            if (headers.Get("Content-Length") != null)
                return System.Int32.Parse(headers.Get("Content-Length"));
            return -1;
        }

        private void DownloadWorker (object sender, System.ComponentModel.DoWorkEventArgs e) {
            Object[] args = (Object[]) e.Argument;
            String tempBase = (String) args[0];
            String[] urls = (String[]) args[1];
            String[] imageNames = (String[]) args[2];
            int[] contentLengths = (int[]) args[3];
            for (int i = 0; i < urls.Length; i++) {
                String asmName = imageNames[i].Substring(imageNames[i].LastIndexOf(@"\") + 1);
                if (!DownloadWorker2(urls[i], imageNames[i], contentLengths[i])) {
                    e.Cancel = true;
                }
            }
            e.Result = args;
        }

        private void DownloadCompleted (object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            pbDownload.Visibility = Visibility.Collapsed;
            MessageListener.Instance.ReceiveMessage("The download is complete...");
            if (e.Cancelled) {
                MessageListener.Instance.ReceiveMessage("but the download images was broken");
            } else if (e.Error != null) {
            } else {
                Object[] args = (Object[]) e.Result;
                String tempBase = (String) args[0];
                String[] urls = (String[]) args[1];
                String[] imageNames = (String[]) args[2];
                bool isVerify = true;
                for (int i = 0; i < imageNames.Length; i++) {
                    if (VerifyAssembly(imageNames[i])) {
                        MessageListener.Instance.ReceiveMessage("verifying...");
                    } else {
                        MessageListener.Instance.ReceiveMessage("Verification failed.");
                        log.Debug(imageNames[i] + " verification failed.");
                        isVerify = false;
                        break;
                    }
                }
                MessageListener.Instance.ReceiveMessage("verification successful.");
                if (isVerify) {
                    String arg = "\"/phase:phase2\" \"/orig:" + _CurrentExeName + "\"";
                    String tempImageLocation = Path.Combine(tempBase, _MyAssembly.GetName().Name + ".exe");
                    log.Debug("Going into " + tempImageLocation + ", arg:" + arg);
                    System.Diagnostics.Process.Start(tempImageLocation, arg);
                    try {
                        Application.Current.MainWindow.Close();
                        System.Windows.Application.Current.Shutdown();
                    } catch (Exception e1) {
                        log.Debug("Whoops!" + e1.ToString());
                    }
                }
            }
        }

        private void Phase2_CopyAndDie () {
            MessageListener.Instance.ReceiveMessage("Phase2_CopyAndDie");
            log.Debug("Phase2_CopyAndDie");
            String orig = CommandLineArgs["orig"];
            String origBase = orig.Substring(0, orig.LastIndexOf(@"\") + 1);
            String currBase = _CurrentExeName.Substring(0, _CurrentExeName.LastIndexOf(@"\"));
            var entries = Directory.EnumerateFileSystemEntries(currBase);
            var fp = new List<String>();
            foreach (var entry in entries) {
                FileInfo entryInfo = new FileInfo(entry);
                if (entryInfo.Extension == ".zip") {
                    ZipFile zip = new ZipFile(entry);
                    zip.ExtractAll(origBase, ExtractExistingFileAction.OverwriteSilently);
                } else {
                    String newEntry = Path.Combine(origBase, entryInfo.Name);
                    File.Copy(entry, newEntry, true);
                    log.Debug("Copying " + entry + " -> " + newEntry);
                }
            }
            String arg = "\"/phase:phase3\" \"/rmdir:" + currBase + "\"";
            log.Debug("Going into orig " + orig + ", arg:" + arg);
            System.Diagnostics.Process.Start(orig, arg);
            try {
                Application.Current.MainWindow.Close();
                System.Windows.Application.Current.Shutdown();
            } catch (Exception e1) {
                log.Debug("Whoops!" + e1.ToString());
            }
        }

        private bool DownloadWorker2 (string url, string imageName, int imageSize) {
            byte[] buf = new byte[2048];
            int n = 0;
            _LastException = null;
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            MarkNoCache(req);
            System.Net.WebResponse resp = req.GetResponse();
            var action = new Action<ProgressBar, int>((p, v) => p.Value = v);
            cancelAction = false;
            using (Stream s1 = new FileStream(imageName, FileMode.CreateNew)) {
                using (Stream s2 = resp.GetResponseStream()) {
                    do {
                        n = s2.Read(buf, 0, buf.Length);
                        // delay, so that the progress bar is visible even for relatively small downloads.
                        if (imageSize < 20000)
                            System.Threading.Thread.Sleep(250);
                        else if (imageSize < 40000)
                            System.Threading.Thread.Sleep(100);
                        else if (imageSize < 100000)
                            System.Threading.Thread.Sleep(50);
                        else if (imageSize < 250000)
                            System.Threading.Thread.Sleep(20);
                        totalBytesTransferred += n;
                        if (n > 0)
                            s1.Write(buf, 0, n);
                        pbDownload.Dispatcher.Invoke(action, pbDownload, totalBytesTransferred);
                    } while (n > 0 && !cancelAction);
                }
            }
            return !cancelAction;
        }

        private void Phase3_MaybeCleanUp () {
            log.Debug("Phase3_MaybeCleanUp");
            if ((CommandLineArgs != null) && (CommandLineArgs.ContainsKey("rmdir"))) {
                string dirToRemove = CommandLineArgs["rmdir"];
                var dirRemover = new System.ComponentModel.BackgroundWorker();
                dirRemover.DoWork += RemoveDirectory;
                dirRemover.RunWorkerAsync(dirToRemove);
            }
        }

        private void RemoveDirectory (object sender, System.ComponentModel.DoWorkEventArgs e) {
            e.Result = false;
            String dirToRemove = (String) e.Argument;
            Exception GotException = null;
            log.Debug("Removing directory " + dirToRemove);
            int Tries = 0;
            do {
                try {
                    Tries++;
                    GotException = null;
                    log.Debug("directory Exists?");
                    if (Directory.Exists(dirToRemove)) {
                        log.Debug("exists, delete it.");
                        Directory.Delete(dirToRemove, true);
                    } else {
                        throw new Exception("wait[" + Tries + "]");
                    }
                } catch (Exception exc) {
                    log.Debug(exc.ToString());
                    GotException = exc;
                    System.Threading.Thread.Sleep(120 * (Tries * Tries + Tries));
                }
            } while ((GotException != null) && (Tries < 7));
            e.Result = (GotException == null);
        }

        private void BuildArgDictionary () {
            string[] args = Environment.GetCommandLineArgs();
            if (args == null)
                return;
            string pattern = @"\/(?<argname>\w+):(?<argvalue>.+)";
            string priorArgname = null;

            CommandLineArgs = new System.Collections.Generic.Dictionary<String, String>();

            for (int i = 1; i < args.Length; i++) {
                string arg = args[i];
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(arg, pattern);

                if (match.Success) {
                    log.Debug("Arg: " + arg);
                    CommandLineArgs.Add(match.Groups["argname"].Value, match.Groups["argvalue"].Value);
                    priorArgname = match.Groups["argname"].Value;
                } else {
                    log.Debug("not Arg?: " + arg);
                }
            }
        }

        private void MarkNoCache (System.Net.WebRequest req) {
            req.CachePolicy = new System.Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore);
        }

        private String HttpGetString (string url) {
            _LastException = null;
            string result = null;
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            MarkNoCache(req);
            try {
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                result = sr.ReadToEnd().Trim();
            } catch (System.Net.WebException ex) {
                throw ex;
            }
            return result;
        }

        private void GetUpdateInfo () {
            _manifestXml = HttpGetString(_manifestUrl);
            if (_manifestXml == null) {
                throw new ApplicationException("No Manifest.");
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(_manifestXml);

            // Verify the signature of the signed XML.
            var csp = new RSACryptoServiceProvider();
            csp.FromXmlString(_publicKeyXml);
            if (!VerifyXml(xmlDoc, csp)) {
                _LastException = new ApplicationException("There is an invalid XML signature on the Manifest.");
                _updateManifest.UpdateIsAvailableAndValid = false;
                return;
            }

            XmlSerializer s = new XmlSerializer(typeof(UpdateManifest));
            using (var sr = new StringReader(_manifestXml)) {
                _updateManifest = (UpdateManifest) s.Deserialize(new System.Xml.XmlTextReader(sr));
                _updateManifest.UpdateIsAvailableAndValid = false;
            }
            return;
        }

        private Boolean VerifyXml (XmlDocument Doc, RSA Key) {
            if (Doc == null)
                return false;
            if (Key == null)
                return false;

            System.Security.Cryptography.Xml.SignedXml signedXml = new SignedXml(Doc);
            XmlNodeList nodeList = Doc.GetElementsByTagName("Signature");
            if (nodeList.Count <= 0) {
                return false;
            }
            if (nodeList.Count >= 2) {
                return false;
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }
            signedXml.LoadXml((XmlElement) nodeList[0]);
            return signedXml.CheckSignature(Key);
        }

        private static String ByteArrayToString (byte[] buffer) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (byte b in buffer)
                sb.Append(b.ToString("X2"));
            return (sb.ToString());
        }

        private bool VerifyHash (String imageName) {
            foreach (SimpleUpdateInfo ui in _updateManifest.Manifest) {
                if (imageName.Substring(imageName.LastIndexOf(@"\") + 1) == ui.ImageName) {
                    _info = ui;
                    break;
                }
            }
            if (_info == null) {
                return false;
            } else {
                if (_info.HashType != GetHashAlgorithm().GetType().ToString()) {
                    return false;
                }
                if (!System.IO.File.Exists(imageName)) {
                    return false;
                }
                byte[] computedHash = ComputeHash(imageName);
                bool result = (ByteArrayToString(computedHash) == ByteArrayToString(_info.Hash));
                return result;
            }
        }

        private bool VerifyAssembly (String imageName) {
            bool result = false;
            try {
                if (!VerifyHash(imageName)) {
                    throw new ApplicationException("Assembly hash mismatch.");
                }
                byte[] chunk = File.ReadAllBytes(imageName);
                var a = System.Reflection.Assembly.Load(chunk);
                if (_info.AssemblyFullName != a.GetName().FullName) {
                    throw new ApplicationException("Assembly name does not match.");
                }
                if (_info.LatestAvailableVersion != ExtractVersion(a.GetName().FullName)) {
                    throw new ApplicationException("Assembly version mismatch.");
                }
                result = true;
            } catch (BadImageFormatException) {
                //check ziped xml
                result = true;
            } catch (Exception ex1) {
                _LastException = ex1;
            }
            return result;
        }

        private static String ExtractVersion (String AssemblyFullName) {
            string pattern = @".+, Version=(?<version>\d+\.\d+\.\d+\.\d+), .+";
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(AssemblyFullName, pattern);
            if (m.Success) {
                if ((m != null) && (m.Captures.Count == 1)) {
                    return m.Groups["version"].ToString();
                }
            }
            return "0.0.0.0";
        }

        private static HashAlgorithm GetHashAlgorithm () {
            return new SHA256CryptoServiceProvider();
        }

        public static byte[] ComputeHash (string Image) {
            byte[] hash = null;
            HashAlgorithm alg = GetHashAlgorithm();
            using (FileStream fs = new FileStream(Image, FileMode.Open, FileAccess.Read)) {
                hash = alg.ComputeHash(fs);
            }
            return hash;
        }
        #endregion

        #region private properties
        private System.Reflection.Assembly _MyAssembly {
            get {
                if (_a == null) {
                    _a = System.Reflection.Assembly.GetEntryAssembly();
                }
                return _a;
            }
        }

        private string _CurrentExeName {
            get {
                String AppExe = _MyAssembly.Location;
                return AppExe;
            }
        }
        #endregion

    }

}
