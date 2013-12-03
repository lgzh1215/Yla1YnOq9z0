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

    public class KCODt {

        private static KCODt instance = null;

        public JToken ShipSpec { get; private set; }
        public JToken ShipData { get; private set; }
        public JToken SlotItem { get; private set; }
        public JToken DeckData { get; private set; }
        public JToken ShipType { get; private set; }
        public Dictionary<string, JToken> ShipSpecMap { get; private set; }
        public Dictionary<string, JToken> SlotItemMap { get; private set; }
        public Dictionary<string, JToken> ShipDataMap { get; private set; }

        public delegate void EventHandler (object sender, DataChangedEventArgs e);

        //public delegate void SlotItemChangedEventHandler (object sender, DataChangedEventArgs e);

        //public delegate void DeckDataChangedEventHandler (object sender, DataChangedEventArgs e);

        public event EventHandler ShipDataChangedEvent;

        //public event SlotItemChangedEventHandler SlotItemChangedEvent;

        //public event DeckDataChangedEventHandler DeckDataChangedEvent;

        static public KCODt Instance {
            get {
                if (instance == null) {
                    instance = new KCODt();
                }
                return instance;
            }
            set {
            }
        }

        protected virtual void OnShipDataChangedEvent (DataChangedEventArgs e) {
            lock (ShipDataMap) {
                ShipData = e.Data;
                ShipDataMap.Clear();
                foreach (JToken sh in ShipData) {
                    ShipDataMap.Add(sh["api_id"].ToString(), sh);
                }
                EventHandler handler = ShipDataChangedEvent;
                if (handler != null) {
                    handler(this, e);
                }
            }
        }

        //protected virtual void OnSlotItemChangedEvent (DataChangedEventArgs e) {
        //    lock (SlotItemMap) {
        //        SlotItem = e.Data;
        //        SlotItemMap.Clear();
        //        foreach (JToken sl in SlotItem) {
        //            ShipDataMap.Add(sl["api_id"].ToString(), sl);
        //        }
        //        SlotItemChangedEventHandler handler = SlotItemChangedEvent;
        //        if (handler != null) {
        //            handler(this, e);
        //        }
        //    }
        //}

        //protected virtual void OnDeckDataChangedEvent (DataChangedEventArgs e) {
        //    DeckData = e.Data;
        //    for (int i = 0; i < DeckData.Count(); i++) {
        //        for (int j = 0; j < DeckData[i]["api_ship"].Count(); j++) {
        //            string key = DeckData[i]["api_ship"][j].ToString();
        //            if (ShipDataMap.ContainsKey(key)) {
        //                JObject jo = ShipDataMap[key] as JObject;
        //                jo.Add("fleet_info", (i + 1) + "-" + (j + 1));
        //            }
        //        }
        //    }
        //    DeckDataChangedEventHandler handler = DeckDataChangedEvent;
        //    if (handler != null) {
        //        handler(this, e);
        //    }
        //}

        private KCODt () {
            InitializeMasterData();
            testMasterData();
            InitializeFiddler();
        }

        private void InitializeMasterData () {
            ShipSpecMap = new Dictionary<string, JToken>();
            ShipDataMap = new Dictionary<string, JToken>();
            SlotItemMap = new Dictionary<string, JToken>();
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
                foreach (JToken sl in SlotItem) {
                    ShipSpecMap.Add(sl["api_id"].ToString(), sl);
                }
            }
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.shiptype.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                ShipType = JToken.Parse(json)["api_data"];
            }
        }

        private void testMasterData () {
            try {
                var qm = from ss in ShipSpec where ss["api_stype"].ToString() == "8" select ss;
                foreach (var qs in qm) {
                    Debug.Print(qs["api_name"].ToString());
                }
            } catch (Exception e) {
                Debug.Print(e.ToString());
            }
        }

        private void InitializeFiddler () {
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
                            OnShipDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            //OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data_deck"]));
                        } catch (Exception exception) {
                            Debug.Print(exception.Message);
                        }
                        break;
                    case "ship3":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            JToken temp = JToken.Parse(json);
                            OnShipDataChangedEvent(new DataChangedEventArgs(temp["api_data"]["api_ship_data"]));
                            //OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data"]["api_deck_data"]));
                        } catch (Exception exception) {
                            Debug.Print(exception.Message);
                        }
                        break;
                    case "deck_port":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            JToken temp = JToken.Parse(json);
                            //OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                        } catch (Exception exception) {
                            Debug.Print(exception.Message);
                        }
                        break;
                    case "slotitem":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            JToken temp = JToken.Parse(json);
                            //OnSlotItemChangedEvent(new DataChangedEventArgs(temp["api_data"]));
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

        private void updateSpecMap () {
            ShipSpecMap.Clear();
            foreach (JToken sh in ShipSpec) {
                ShipSpecMap.Add(sh["api_id"].ToString(), sh);
            }
        }

    }

    public class DataChangedEventArgs : EventArgs {

        private readonly JToken data;

        public DataChangedEventArgs (JToken data) {
            this.data = data;
        }

        public JToken Data {
            get { return this.data; }
        }
    }
}
