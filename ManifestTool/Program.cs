using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using Ionic.Zip;
using System.Reflection;
using Launcher.AppUpdater;
using AVSyncer.Movies;
using System.Text.RegularExpressions;
using System.Collections;

namespace Ionic.AppUpdater {
	enum DesiredAction {
		None = 0,
		EmitPk,
		Directory,
	}

	static class Extensions {
		public static string SerializeToString (this XmlSerializer s, object o, string ns) {
			var n = new XmlSerializerNamespaces();
			n.Add("", ns);
			var builder = new System.Text.StringBuilder();
			var settings = new System.Xml.XmlWriterSettings {
				OmitXmlDeclaration = true,
				Indent = true
			};

			using (var writer = System.Xml.XmlWriter.Create(builder, settings)) {
				s.Serialize(writer, o, n);
			}
			return builder.ToString();
		}

	}


	class ManifestTool {
		public ManifestTool (string[] args) {
			for (int i = 0; i < args.Length; i++) {
				switch (args[i]) {
					case "-d":
						i++;
						if (args.Length <= i)
							throw new ArgumentException(args[i]);
						_dir = args[i++];
						_url = args[i];
						if (_desiredAction != DesiredAction.None)
							throw new ArgumentException(args[i]);
						_desiredAction = DesiredAction.Directory;
						break;

					case "-k":
						i++;
						if (args.Length <= i)
							throw new ArgumentException(args[i]);
						_snkFile = args[i];
						break;

					case "-o":
						i++;
						if (args.Length <= i)
							throw new ArgumentException(args[i]);
						_manifestFile = args[i];
						break;

					case "-pk":
						i++;
						if (_desiredAction != DesiredAction.None)
							throw new ArgumentException(args[i]);
						_desiredAction = DesiredAction.EmitPk;
						break;

					case "-?":
						throw new ArgumentException(args[i]);

					case ">":
						i++;
						break;

					default:
						throw new ArgumentException(args[i]);

				}
			}

			// validate
			if (_desiredAction == DesiredAction.EmitPk && _snkFile == null)
				throw new ArgumentException("-pk");

		}

		public void Run () {
			switch (_desiredAction) {

				case DesiredAction.None:
					Usage();
					break;

				case DesiredAction.EmitPk:
					EmitPublicKey();
					break;

				case DesiredAction.Directory:
					ReadDirectory();
					GenerateDirectoryManifest();
					break;

			}
		}

		private void EmitPublicKey () {
			var rsa = SnkUtil.GetRsaProvider(_snkFile);
			Console.WriteLine("{0}", rsa.ToXmlString(false));
		}

		private void ReadDirectory () {
			if (!Directory.Exists(_dir)) {
				throw new Exception("Directory doesn't exists!");
			}
			_updateManifest = new UpdateManifest();
			_updateManifest.Manifest = new List<SimpleUpdateInfo>();
			if (File.Exists(_dir + ".zip")) {
			    File.Delete(_dir + ".zip");
			}
			List<String> archivedImages = new List<String>();
			ZipFile zip = new ZipFile(_dir + ".zip");
			String[] ave = {
					"AVSyncer.exe$",
					".dll",
					"uncAV.xml$"
			};
			String[] force = {
                    "log4net.dll",
					"Ionic.Zip.Reduced.dll"
			};
			var entries = Directory.EnumerateFileSystemEntries(_dir);
			var fp = new List<String>();
			foreach (var entry in entries) {
				Boolean isRecognize = false;
				Boolean isForceUpdate = false;
				foreach (String p in ave) {
					foreach (String s in matches(p, entry, 0)) {
						isRecognize = true;
						break;
					}
					if (isRecognize) {
						break;
					}
				}
				foreach (String p in force) {
					foreach (String s in matches(p, entry, 0)) {
						isForceUpdate = true;
						break;
					}
					if (isForceUpdate) {
						break;
					}
				}
				if (isRecognize && File.Exists(entry)) {
					byte[] chunk = File.ReadAllBytes(entry);
					Assembly a = null;
					SimpleUpdateInfo info = null;
					try {
						a = System.Reflection.Assembly.Load(chunk);
						info = new SimpleUpdateInfo() {
							Archived = false,
							AssemblyFullName = a.GetName().FullName,
							ImageName = Path.GetFileName(entry),
							LatestAvailableVersion = ExtractVersion(a.GetName().FullName),
							Hash = ComputeHash(entry),
							HashType = GetHashAlgorithm().GetType().ToString(),
							TimeStamp = File.GetLastWriteTime(entry),
							ForceUpdate = isForceUpdate
						};
						zip.AddFile(entry, "CurrentVersion");
					} catch (BadImageFormatException) {
						FileInfo file = new FileInfo(entry);
						String ImageName = entry + ".zip";
						archivedImages.Add(ImageName);
						if (File.Exists(ImageName)) {
							File.Delete(ImageName);
						}
						ZipFile unczip = new ZipFile(ImageName);
						unczip.AddFile(entry, "");
						unczip.Save();
						if (file.Name == "uncAV.xml") {
							MovieDatas md = new MovieDatas().Deserialize(entry);
							info = new SimpleUpdateInfo() {
								Archived = true,
								AssemblyFullName = file.Name + ".zip, version=" + md.Version,
								ImageName = Path.GetFileName(entry) + ".zip",
								LatestAvailableVersion = md.Version,
								Hash = ComputeHash(ImageName),
								HashType = GetHashAlgorithm().GetType().ToString(),
								TimeStamp = File.GetLastWriteTime(entry),
								ForceUpdate = isForceUpdate
							};
							zip.AddFile(ImageName, "CurrentVersion");

						} else {
							info = new SimpleUpdateInfo() {
								Archived = false,
								AssemblyFullName = file.Name,
								ImageName = Path.GetFileName(entry),
								LatestAvailableVersion = "0.0.0.0",
								Hash = ComputeHash(entry),
								HashType = GetHashAlgorithm().GetType().ToString(),
								TimeStamp = File.GetLastWriteTime(entry),
								ForceUpdate = isForceUpdate
							};
						}
					} catch (Exception e) {
						System.Console.WriteLine(e.Message);
					}
					_updateManifest.Manifest.Add(info);
				}
			}
			zip.Save();
			foreach (String image in archivedImages) {
				File.Delete(image);
			}
			_updateManifest.CodeBase = _url;
		}

		private void GenerateDirectoryManifest () {
			string assemblyPath = null;
			var rsa = SnkUtil.GetRsaProvider(_snkFile);
			try {
				XmlSerializer s = new XmlSerializer(typeof(UpdateManifest));
				string manifest = s.SerializeToString(_updateManifest, "urn:AVSyncer.Updates");
				var signedManifestDoc = SignManifest(manifest);

				var sb = new System.Text.StringBuilder();
				var settings = new System.Xml.XmlWriterSettings {
					OmitXmlDeclaration = true,
					Indent = true
				};
				using (var writer = System.Xml.XmlWriter.Create(sb, settings)) {
					signedManifestDoc.Save(writer);
				}
				StreamWriter sw = new StreamWriter(_manifestFile);
				sw.Write(sb.ToString());
				sw.Close();
				ZipFile zip = new ZipFile(_dir + ".zip");
				zip.AddFile(_manifestFile, "");
				zip.Save();
			} finally {
				if (assemblyPath != null && File.Exists(assemblyPath))
					File.Delete(assemblyPath);
			}
		}

		#region oldcode
		// Verify the signature of an XML file against an asymmetric 
		// algorithm and return the result.
		private Boolean VerifyXml (XmlDocument Doc, RSA Key) {
			// Check arguments.
			if (Doc == null)
				throw new ArgumentException("Doc");
			if (Key == null)
				throw new ArgumentException("Key");

			// Create a new SignedXml object and pass it
			// the XML document class.
			var signedXml = new System.Security.Cryptography.Xml.SignedXml(Doc);

			// Find the "Signature" node and create a new XmlNodeList object.
			XmlNodeList nodeList = Doc.GetElementsByTagName("Signature");

			// Throw an exception if no signature was found.
			if (nodeList.Count <= 0) {
				throw new CryptographicException("Verification failed: No Signature was found in the document.");
			}

			// Though it is possible to have multiple signatures on 
			// an XML document, this app only supports one signature for
			// the entire XML document.  Throw an exception 
			// if more than one signature was found.
			if (nodeList.Count >= 2) {
				throw new CryptographicException("Verification failed: More that one signature was found for the document.");
			}

			// Load the first <signature> node.  
			signedXml.LoadXml((XmlElement)nodeList[0]);

			// Check the signature and return the result.
			return signedXml.CheckSignature(Key);
		}

		private string DownloadImage () {
			byte[] buf = new byte[2048];
			int n = 0;
			string tempLocation = GetTempLocation();

			System.Net.WebRequest req = System.Net.WebRequest.Create(_url);
			System.Net.WebResponse resp = req.GetResponse();
			int totalBytesTransferred = 0;

			using (Stream s1 = new FileStream(tempLocation, FileMode.CreateNew)) {
				using (Stream s2 = resp.GetResponseStream()) {
					do {
						n = s2.Read(buf, 0, buf.Length);
						totalBytesTransferred += n;
						if (n > 0)
							s1.Write(buf, 0, n);
					} while (n > 0);
					Console.WriteLine();
				}
			}
			return tempLocation;
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

		private bool VerifyHash (byte[] hash, string hashType, string ImageName) {
			if (hashType != GetHashAlgorithm().GetType().ToString())
				return false;

			if (!File.Exists(ImageName))
				return false;

			byte[] computedHash = ComputeHash(ImageName);

			return (ByteArrayToString(computedHash) == ByteArrayToString(hash));
		}

		private static string ByteArrayToString (byte[] buffer) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (byte b in buffer)
				sb.Append(b.ToString("X2"));

			return (sb.ToString());
		}

		private static string ExtractVersion (string AssemblyFullName) {
			string pattern = @".+, Version=(?<version>\d+\.\d+\.\d+\.\d+), .+";
			System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(AssemblyFullName, pattern);

			if (m.Success) {
				if ((m != null) && (m.Captures.Count == 1)) {
					return m.Groups["version"].ToString();
				}
			}
			return "0.0.0.0";
		}

		private string GetTempLocation () {
			string tempLocation = null;
			String appName = _MyAssembly.GetName().Name;
			string tempDir = System.Environment.GetEnvironmentVariable("TEMP");
			if (tempDir == null)
				tempDir = ".";
			do {
				if (tempLocation != null)
					System.Threading.Thread.Sleep(200);
				string n = String.Format("{0}-{1}.tmp", appName, System.DateTime.Now.ToString("yyyyMMMdd-HHmmss"));
				tempLocation = System.IO.Path.Combine(tempDir, n);
			} while (System.IO.File.Exists(tempLocation));

			return tempLocation;
		}

		public System.Xml.XmlDocument SignManifest (string rawXml) {
			System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();

			// Load an XML file into the XmlDocument object.
			xmlDoc.PreserveWhitespace = true;
			xmlDoc.LoadXml(rawXml);
			var rsa = SnkUtil.GetRsaProvider(_snkFile);

			// Sign the XML document. 
			SignXml(xmlDoc, rsa);
			return xmlDoc;
		}

		// Sign an XML file. 
		// This document cannot be verified unless the verifying 
		// code has the key with which it was signed.
		public static void SignXml (System.Xml.XmlDocument Doc, RSA Key) {
			// Check arguments.
			if (Doc == null)
				throw new ArgumentException("Doc");
			if (Key == null)
				throw new ArgumentException("Key");

			// Create a SignedXml object.
			var signedXml = new System.Security.Cryptography.Xml.SignedXml(Doc);

			// Add the key to the SignedXml document.
			signedXml.SigningKey = Key;

			// Create a reference to be signed.
			var reference = new System.Security.Cryptography.Xml.Reference();
			reference.Uri = "";

			// Add an enveloped transformation to the reference.
			var env = new System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform();
			reference.AddTransform(env);

			// Add the reference to the SignedXml object.
			signedXml.AddReference(reference);

			// Compute the signature.
			signedXml.ComputeSignature();

			// Get the XML representation of the signature and save
			// it to an XmlElement object.
			XmlElement xmlDigitalSignature = signedXml.GetXml();

			// Append the element to the XML document.
			Doc.DocumentElement.AppendChild(Doc.ImportNode(xmlDigitalSignature, true));
		}

		private System.Reflection.Assembly _MyAssembly {
			get {
				if (_a == null) {
					_a = System.Reflection.Assembly.GetExecutingAssembly();
				}
				return _a;
			}
		}
		#endregion

		static void Main (string[] args) {
			try {
				String mbase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\");
				String dir = Path.Combine(mbase, @"AVSyncer\bin\Release");
				String snk = Path.Combine(mbase, @"AVSyncer.snk");
				String Out = Path.Combine(mbase, @"Manifest.xml");
				String[] arg = {
					"-d",
					dir,
					"http://ghchen.twbbs.org/AVSyncer/CurrentVersion",
					"-k",
					snk,
					"-o",
					Out,
				};
				new ManifestTool(arg).Run();
				File.Copy(dir + ".zip", Path.Combine(mbase, "Release.zip"), true);
				File.Delete(dir + ".zip");
				File.Delete(Out);
			} catch (System.Exception exc1) {
				Console.WriteLine("uncaught exception: {0}", exc1);
			}
		}

		public static IEnumerable matches (String pPattern, String pText, int pGroupId) {
			Regex r = new Regex(pPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			for (Match m = r.Match(pText); m.Success; m = m.NextMatch())
				yield return m.Groups[pGroupId].Value;
		}

		public static void Usage () {
			string helpString = @"
ManifestTool: produces a manifest for the specified assembly, suitable for
  use with Ionic's AppUpdater module.

Usage:
   ManifestTool -k <snkFile> -g <uri>
       generate a manifest.  (Redirect the output to create a file).
       The URI is a http location that holds the assembly.

   ManifestTool -k <snkFile> -pk
       emits the public Key XML for the given SNK file.  This is usable
       within the WPF application that uses Ionic's AppUpdater.

   ManifestTool -k <snkFile> -v <manifest>
       verify the manifest.  Checks that an Assembly is downloadable from the location
       specified in the manifest, and that the hash of the downloaded image matches
       the hash specified in the manifest.

   ManifestTool -k <snkFile> -d <directory> -gd <uri>
";
			Console.WriteLine(helpString);
		}


		private System.Reflection.Assembly _a;
		private string _snkFile;
		private string _url;
		private DesiredAction _desiredAction = DesiredAction.None;
		private string _dir;
		private UpdateManifest _updateManifest;
		private string _manifestFile;
	}
}
