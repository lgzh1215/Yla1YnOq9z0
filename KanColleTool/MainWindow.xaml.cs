using System;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Threading;
using Fiddler;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace KanColleTool {
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window {

        dynamic ship;
        dynamic deckPort;

        public MainWindow () {
            InitializeComponent();
            ship = initShipData();
            InitializeFiddler();
        }

        private object initShipData () {
            Assembly assembly = typeof(MainWindow).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.ship.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                return JObject.Parse(json);
            }
        }

        public delegate void KanColleInvoker ();

        private void InitializeFiddler () {

            Thread UIThread = Thread.CurrentThread;
            MainWindow mainWindow = this;
            string pattern = @".*\/kcsapi\/.*\/(.*)";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Fiddler.FiddlerApplication.OnNotification += delegate(object sender, NotificationEventArgs oNEA) { Console.WriteLine("** NotifyUser: " + oNEA.NotifyString); };
            Fiddler.FiddlerApplication.Log.OnLogString += delegate(object sender, LogEventArgs oLEA) { Console.WriteLine("** LogString: " + oLEA.LogString); };

            Fiddler.FiddlerApplication.BeforeRequest += delegate(Fiddler.Session oS) {
                if (r.IsMatch(oS.fullUrl)) {
                    NameValueCollection form = HttpUtility.ParseQueryString(oS.GetRequestBodyAsString());
                    string token = form["api_token"];
                    Dispatcher.FromThread(UIThread).Invoke((MainWindow.KanColleInvoker) delegate {
                        if (token != null && !mainWindow.textToken.Text.Equals(token)) {
                            mainWindow.textToken.Text = token;
                        }
                    }, null);
                    oS.bBufferResponse = false;
                }
            };

            Fiddler.FiddlerApplication.AfterSessionComplete += delegate(Fiddler.Session oS) {
                if (!r.IsMatch(oS.fullUrl)) {
                    return;
                }
                Match m = Regex.Match(oS.fullUrl, pattern);
                Console.WriteLine("Finished session:\t" + oS.fullUrl);
                switch (m.Groups[1].ToString()) {
                    case "deck_port":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            deckPort = JObject.Parse(json);

                            long unixDate = deckPort.api_data[1].api_mission[2];
                            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                            DateTime date = start.AddMilliseconds(unixDate).ToLocalTime();
                            Dispatcher.FromThread(UIThread).Invoke((MainWindow.KanColleInvoker) delegate {
                                //mainWindow.label1.Content = deckPort.api_data[0].api_name;
                                mainWindow.label1.Content = date.ToString("HH:mm:ss");
                            }, null);
                        } catch (Exception exception) {
                            Console.Write(exception.Message);
                        }
                        break;
                    default:
                        break;
                }
                Console.Write(String.Format("{0} {1}\n{2} {3}\n\n", oS.id, oS.oRequest.headers.HTTPMethod, oS.responseCode, oS.oResponse.MIMEType));
            };

            //Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            string sSAZInfo = "NoSAZ";
            Console.WriteLine(String.Format("Starting {0} ({1})...", Fiddler.FiddlerApplication.GetVersionString(), sSAZInfo));
            Fiddler.CONFIG.IgnoreServerCertErrors = false;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);

            FiddlerCoreStartupFlags oFCSF = FiddlerCoreStartupFlags.Default;
            oFCSF = (oFCSF & ~FiddlerCoreStartupFlags.DecryptSSL);

            int iPort = 8877;
            Fiddler.FiddlerApplication.Startup(iPort, oFCSF);

            FiddlerApplication.Log.LogFormat("Created endpoint listening on port {0}", iPort);
            FiddlerApplication.Log.LogFormat("Starting with settings: [{0}]", oFCSF);
            FiddlerApplication.Log.LogFormat("Gateway: {0}", CONFIG.UpstreamGateway.ToString());

        }

        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            Console.WriteLine("Shutting down...");
            Fiddler.FiddlerApplication.Shutdown();
            Thread.Sleep(500);
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

    }
}
