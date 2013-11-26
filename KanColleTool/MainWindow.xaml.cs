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
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window {

        JObject ship;
        JObject ship2;
        JObject slotitem;
        JObject deckport;
        Dictionary<string, JToken> shipMap = new Dictionary<string, JToken>();
        Dictionary<string, JToken> ship2Map = new Dictionary<string, JToken>();
        RequestBuilder requestBuilder;
        Timer UITimer;
        Thread UIThread;
        public delegate void KanColleInvoker ();

        public MainWindow () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
            InitializeMasterData();
            testMasterData();
            InitializeFiddler();
            InitializeTimer();
        }

        void InitializeMasterData () {
            Assembly assembly = typeof(MainWindow).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.ship.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                ship = JObject.Parse(json);
                foreach (JToken sh in ship["api_data"]) {
                    shipMap.Add(sh["api_id"].ToString(), sh);
                }
            }
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.slotitem.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                slotitem = JObject.Parse(json);
            }
        }

        void testMasterData () {
            try {
                var qm = from ss in ship["api_data"] where ss["api_stype"].ToString() == "4" select ss;
                foreach (var qs in qm) {
                    Debug.Print(qs["api_name"].ToString());
                }
            } catch (Exception e) {
                Debug.Print(e.ToString());
            }
        }

        void InitializeTimer () {
            TimerCallback tcb = this.update;
            UITimer = new Timer(tcb, null, 0, 1000);
        }

        public void update (Object context) {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.KanColleInvoker) delegate {
                try {
                    JToken jdeck = null;
                    if (ship2 != null) {
                        jdeck = ship2["api_data_deck"];
                    } else if (deckport != null) {
                        jdeck = deckport["api_data"];
                    }
                    if (jdeck == null) {
                        return;
                    }
                    labFl1Name.Content = jdeck[0]["api_name"].ToString();
                    // 2
                    labFl2Name.Content = jdeck[1]["api_name"].ToString();
                    labFl2MissionETA.Content = valueOfUTC(jdeck[1]["api_mission"][2].ToString());
                    TimeSpan span2 = countSpan(jdeck[1]["api_mission"][2].ToString());
                    labFl2MissionCD.Content = span2.ToString(@"hh\:mm\:ss");
                    // 3
                    labFl3Name.Content = jdeck[2]["api_name"].ToString();
                    labFl3MissionETA.Content = valueOfUTC(jdeck[2]["api_mission"][2].ToString());
                    TimeSpan span3 = countSpan(jdeck[2]["api_mission"][2].ToString());
                    labFl3MissionCD.Content = span3.ToString(@"hh\:mm\:ss");
                    // 4
                    labFl4Name.Content = jdeck[3]["api_name"].ToString();
                    labFl4MissionETA.Content = valueOfUTC(jdeck[3]["api_mission"][2].ToString());
                    TimeSpan span4 = countSpan(jdeck[3]["api_mission"][2].ToString());
                    labFl4MissionCD.Content = span4.ToString(@"hh\:mm\:ss");
                    if (requestBuilder != null && !requestBuilder.OnInvoke) {
                        button1.IsEnabled = true;
                    }
                } catch (Exception e) {
                    Debug.Print(e.ToString());
                }
            }, null);
        }

        TimeSpan countSpan (string value) {
            DateTime eta = parseUTC(value);
            TimeSpan span = eta - DateTime.Now;
            return span;
        }

        string valueOfUTC (string value) {
            string result = "";
            if (value != "" && value != "0") {
                DateTime date = parseUTC(value);
                result = date.ToString("HH:mm:ss");
            }
            return result;
        }

        DateTime parseUTC (string value) {
            long utc = long.Parse(value);
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(utc).ToLocalTime();
            return date;
        }

        void InitializeFiddler () {
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
                Dispatcher.FromThread(UIThread).Invoke((MainWindow.KanColleInvoker) delegate {
                    if (token != null && !this.textToken.Text.Equals(token)) {
                        this.textToken.Text = token;
                        requestBuilder = new RequestBuilder(oS);
                    }
                }, null);
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
                            ship2 = JObject.Parse(json);
                            ship2Map.Clear();
                            foreach (JToken sh in ship2["api_data"]) {
                                ship2Map.Add(sh["api_id"].ToString(), sh);
                            }
                        } catch (Exception exception) {
                            Debug.Print(exception.Message);
                        }
                        break;
                    case "deck_port":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            deckport = JObject.Parse(json);
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

        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            Debug.Print("Shutting down...");
            Fiddler.FiddlerApplication.Shutdown();
            Thread.Sleep(500);
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void btnFl2Result_Click (object sender, RoutedEventArgs e) {
            try {
                requestBuilder.MissionReturn(2);
            } catch (Exception ex) {
                Debug.Print(ex.Message);
            }
        }

        private void btnFl3Result_Click (object sender, RoutedEventArgs e) {
            try {
                requestBuilder.MissionReturn(3);
            } catch (Exception ex) {
                Debug.Print(ex.Message);
            }
        }

        private void btnFl4Result_Click (object sender, RoutedEventArgs e) {
            try {
                requestBuilder.MissionReturn(4);
            } catch (Exception ex) {
                Debug.Print(ex.Message);
            }
        }

        private void button1_Click (object sender, RoutedEventArgs e) {
            try {
                for (int i = 0; i < 4; i++) {
                    ICollection<string> chargeIds = listChargeShips(i);
                    requestBuilder.FleetCharge(i, chargeIds);
                }
            } catch (Exception ex) {
                Debug.Print(ex.Message);
            }
        }

        private ICollection<string> listChargeShips (int fleet) {
            List<JToken> shipIds = ship2["api_data_deck"][fleet]["api_ship"].ToList();
            List<string> tgtShipIds = new List<string>();
            HashSet<string> chargeIds = new HashSet<string>();
            try {
                foreach (var shipId in shipIds) {
                    if (shipId.ToString() != "-1") {
                        tgtShipIds.Add(shipId.ToString());
                    }
                }
                var qs = from sh in ship["api_data"]
                         from s2 in ship2["api_data"]
                         where
                             tgtShipIds.Contains(s2["api_id"].ToString()) &&
                             sh["api_id"].ToString() == s2["api_ship_id"].ToString()
                         select sh;
                foreach (var sid in tgtShipIds) {
                    if (!ship2Map.ContainsKey(sid)) {
                        continue;
                    }
                    JToken myShip = ship2Map[sid];
                    JToken defShip = shipMap[myShip["api_ship_id"].ToString()];
                    string msg = defShip["api_name"].ToString();
                    msg += "\t\tF: " + myShip["api_fuel"].ToString() + "/" + defShip["api_fuel_max"].ToString();
                    if (myShip["api_fuel"].ToString() != defShip["api_fuel_max"].ToString()) {
                        chargeIds.Add(sid);
                        msg += "*";
                    }
                    msg += "\tB: " + myShip["api_bull"].ToString() + "/" + defShip["api_bull_max"].ToString();
                    if (myShip["api_bull"].ToString() != defShip["api_bull_max"].ToString()) {
                        chargeIds.Add(sid);
                        msg += "x";
                    }
                    Debug.Print(msg);
                }
            } catch (Exception ex) {
                Debug.Print(ex.Message);
            }
            return chargeIds;
        }

        private void btnFl1Kira_Click (object sender, RoutedEventArgs e) {

        }

    }
}
