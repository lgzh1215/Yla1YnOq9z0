using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Documents;
using Fiddler;
using System.Windows.Threading;

namespace KanColleTool {
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow () {
            InitializeComponent();
            InitializeFiddler();
        }

        public delegate void KanColleInvoker ();

        private void InitializeFiddler () {
            Thread UIThread = Thread.CurrentThread;
            MainWindow mainWindow = this;
            string pattern = @".*\/kcsapi\/.*\/(.*)";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            //List<Fiddler.Session> oAllSessions = new List<Fiddler.Session>();
            #region AttachEventListeners

            Fiddler.FiddlerApplication.OnNotification += delegate(object sender, NotificationEventArgs oNEA) { Console.WriteLine("** NotifyUser: " + oNEA.NotifyString); };
            Fiddler.FiddlerApplication.Log.OnLogString += delegate(object sender, LogEventArgs oLEA) { Console.WriteLine("** LogString: " + oLEA.LogString); };

            Fiddler.FiddlerApplication.BeforeRequest += delegate(Fiddler.Session oS) {
                if (r.IsMatch(oS.fullUrl)) {
                    Console.WriteLine("Before request for:\t" + oS.fullUrl);
                    Console.WriteLine("Before request body:\t" + oS.GetRequestBodyAsString());
                    NameValueCollection form = HttpUtility.ParseQueryString(oS.GetRequestBodyAsString());
                    string token = form["api_token"];
                    Dispatcher.FromThread(UIThread).Invoke((MainWindow.KanColleInvoker) delegate {
                        if (token != null && !mainWindow.textToken.Text.Equals(token)) {
                            mainWindow.textToken.Text = token;
                        }
                    }, null);

                    oS.bBufferResponse = false;
                    //Monitor.Enter(oAllSessions);
                    //oAllSessions.Add(oS);
                    //Monitor.Exit(oAllSessions);
                }
            };

            Fiddler.FiddlerApplication.AfterSessionComplete += delegate(Fiddler.Session oS) {
                if (r.IsMatch(oS.fullUrl)) {
                    Console.WriteLine("Finished session:\t" + oS.fullUrl);
                    Console.Write(String.Format("{0} {1}\n{2} {3}\n\n", oS.id, oS.oRequest.headers.HTTPMethod, oS.responseCode, oS.oResponse.MIMEType));
                }
            };

            //Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            #endregion AttachEventListeners

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
