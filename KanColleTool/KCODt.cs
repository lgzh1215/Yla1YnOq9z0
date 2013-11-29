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

        static public JToken ShipSpec { get; private set; }
        static public JToken ShipData { get; private set; }
        static public JToken SlotItem { get; private set; }
        static public JToken DeckData { get; private set; }
        static public JToken ShipType { get; private set; }
        static public Dictionary<string, JToken> ShipSpecMap { get; private set; }
        static public Dictionary<string, JToken> ShipDataMap { get; private set; }

        static public void InitializeObserver () {
            InitializeMasterData();
            testMasterData();
            InitializeFiddler();
        }

        static void InitializeMasterData () {
            ShipSpecMap = new Dictionary<string, JToken>();
            ShipDataMap = new Dictionary<string, JToken>();
            Assembly assembly = typeof(MainWindow).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.ship.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                ShipSpec = JToken.Parse(json)["api_data"];
                foreach (JToken sh in ShipSpec) {
                    ShipSpecMap.Add(sh["api_id"].ToString(), sh);
                }
            }
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.slotitem.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                SlotItem = JToken.Parse(json)["api_data"];
            }
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.shiptype.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                ShipType = JToken.Parse(json)["api_data"];
            }
        }

        static void testMasterData () {
            try {
                var qm = from ss in ShipSpec where ss["api_stype"].ToString() == "8" select ss;
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
                oS.bBufferResponse = false;
            };

            Fiddler.FiddlerApplication.AfterSessionComplete += delegate(Fiddler.Session oS) {
                if (!r.IsMatch(oS.fullUrl)) {
                    return;
                }
                Match m = Regex.Match(oS.fullUrl, pattern);
                Debug.Print(String.Format("{0:hh:mm:ss.fff}\tEd session:\t{1}", DateTime.Now, oS.fullUrl));
                switch (m.Groups[1].ToString()) {
                    case "ship":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            ShipSpec = JToken.Parse(json)["api_data"];
                            updateSpecMap();
                        } catch (Exception exception) {
                            Debug.Print(exception.Message);
                        }
                        break;
                    case "ship2":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            JToken temp = JToken.Parse(json);
                            ShipData = temp["api_data"];
                            updateShipMap();
                            updateDeckInfo(temp["api_data_deck"]);
                        } catch (Exception exception) {
                            Debug.Print(exception.Message);
                        }
                        break;
                    case "ship3":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            JToken temp = JToken.Parse(json);
                            ShipData = temp["api_data"]["api_ship_data"];
                            SlotItem = temp["api_data"]["api_slot_data"];
                            updateShipMap();
                            updateDeckInfo(temp["api_data"]["api_deck_data"]);
                        } catch (Exception exception) {
                            Debug.Print(exception.Message);
                        }
                        break;
                    case "deck_port":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            updateDeckInfo(JToken.Parse(json)["api_data"]);
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

        private static void updateSpecMap () {
            ShipSpecMap.Clear();
            foreach (JToken sh in ShipSpec) {
                ShipSpecMap.Add(sh["api_id"].ToString(), sh);
            }
        }

        static private void updateShipMap () {
            ShipDataMap.Clear();
            foreach (JToken sh in ShipData) {
                ShipDataMap.Add(sh["api_id"].ToString(), sh);
            }
        }

        private static void updateDeckInfo (JToken data) {
            DeckData = data;
            for (int i = 0; i < DeckData.Count(); i++) {
                for (int j = 0; j < DeckData[i]["api_ship"].Count(); j++) {
                    string key = DeckData[i]["api_ship"][j].ToString();
                    if (ShipDataMap.ContainsKey(key)) {
                        JObject jo = ShipDataMap[key] as JObject;
                        jo.Add("fleet_info", (i + 1) + "-" + (j + 1));
                    }
                }
            }
        }
    }
}
