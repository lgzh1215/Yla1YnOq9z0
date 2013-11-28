using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Threading;
using Fiddler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;

namespace KanColleTool {

    static public class KCODt {

        static public JObject Ship { get; private set; }
        static public JObject Ship2 { get; private set; }
        static public JObject SlotItem { get; private set; }
        static public JObject DeckPort { get; private set; }
        static public Dictionary<string, JToken> ShipMap { get; private set; }
        static public Dictionary<string, JToken> Ship2Map { get; private set; }

        static public void InitializeObserver () {
            InitializeMasterData();
            testMasterData();
            InitializeFiddler();
        }

        static void InitializeMasterData () {
            ShipMap = new Dictionary<string, JToken>();
            Ship2Map = new Dictionary<string, JToken>();
            Assembly assembly = typeof(MainWindow).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.ship.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                Ship = JObject.Parse(json);
                foreach (JToken sh in Ship["api_data"]) {
                    ShipMap.Add(sh["api_id"].ToString(), sh);
                }
            }
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.slotitem.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                SlotItem = JObject.Parse(json);
            }
        }

        static void testMasterData () {
            try {
                var qm = from ss in Ship["api_data"] where ss["api_stype"].ToString() == "4" select ss;
                foreach (var qs in qm) {
                    Debug.Print(qs["api_name"].ToString());
                }
            } catch (Exception e) {
                Debug.Print(e.ToString());
            }
        }

        static void InitializeFiddler () {
            string pattern = @".*\/kcsapi\/.*\/(.*)";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Fiddler.FiddlerApplication.OnNotification += delegate(object sender, NotificationEventArgs oNEA) { Debug.Print("** NotifyUser: " + oNEA.NotifyString); };
            Fiddler.FiddlerApplication.Log.OnLogString += delegate(object sender, LogEventArgs oLEA) { Debug.Print("** LogString: " + oLEA.LogString); };

            Fiddler.FiddlerApplication.BeforeRequest += delegate(Fiddler.Session oS) {
                if (!r.IsMatch(oS.fullUrl)) {
                    return;
                }
                //Debug.Print(String.Format("{0:hh:mm:ss.fff}\tOp session:\t{1}", DateTime.Now, oS.fullUrl));
                NameValueCollection form = HttpUtility.ParseQueryString(oS.GetRequestBodyAsString());
                string token = form["api_token"];
                if (token != null && RequestBuilder.Token == null) {
                    RequestBuilder.Initialize(oS);
                } else if (token != null && !RequestBuilder.Token.Equals(token)) {
                    RequestBuilder.Initialize(oS);
                }
                //requestBuilder = new RequestBuilder(oS);
                //Dispatcher.FromThread(UIThread).Invoke((MainWindow.KanColleInvoker) delegate {
                //    if (token != null && !this.textToken.Text.Equals(token)) {
                //        this.textToken.Text = token;
                //        requestBuilder = new RequestBuilder(oS);
                //    }
                //}, null);
                oS.bBufferResponse = false;
            };

            Fiddler.FiddlerApplication.AfterSessionComplete += delegate(Fiddler.Session oS) {
                if (!r.IsMatch(oS.fullUrl)) {
                    return;
                }
                Match m = Regex.Match(oS.fullUrl, pattern);
                Debug.Print(String.Format("{0:hh:mm:ss.fff}\tEd session:\t{1}", DateTime.Now, oS.fullUrl));
                switch (m.Groups[1].ToString()) {
                    case "ship2":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            Ship2 = JObject.Parse(json);
                            Ship2Map.Clear();
                            foreach (JToken sh in Ship2["api_data"]) {
                                Ship2Map.Add(sh["api_id"].ToString(), sh);
                            }
                        } catch (Exception exception) {
                            Debug.Print(exception.Message);
                        }
                        break;
                    case "deck_port":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            DeckPort = JObject.Parse(json);
                        } catch (Exception exception) {
                            Debug.Print(exception.Message);
                        }
                        break;
                    default:
                        break;
                }
            };
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
    }
}
