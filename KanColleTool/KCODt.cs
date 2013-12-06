﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Fiddler;
using Newtonsoft.Json.Linq;

namespace KanColleTool {

    public class KCODt {

        private static KCODt instance = null;

        public JToken ShipSpec { get; private set; }
        public JToken ShipData { get; private set; }
        public JToken ItemSpec { get; private set; }
        public JToken ItemData { get; private set; }
        public JToken DeckData { get; private set; }
        public JToken ShipType { get; private set; }
        public Dictionary<string, JToken> ShipSpecMap { get; private set; }
        public Dictionary<string, JToken> ShipDataMap { get; private set; }
        public Dictionary<string, JToken> ItemDataMap { get; private set; }
        public Dictionary<string, JToken> ItemSpecMap { get; private set; }

        public delegate void ShipSpecChangedEventHandler (object sender, DataChangedEventArgs e);

        public delegate void ShipDataChangedEventHandler (object sender, DataChangedEventArgs e);

        public delegate void ItemSpecChangedEventHandler (object sender, DataChangedEventArgs e);

        public delegate void ItemDataChangedEventHandler (object sender, DataChangedEventArgs e);

        public delegate void DeckDataChangedEventHandler (object sender, DataChangedEventArgs e);

        public event ShipSpecChangedEventHandler ShipSpecChanged;

        public event ShipDataChangedEventHandler ShipDataChanged;

        public event ItemSpecChangedEventHandler ItemSpecChanged;

        public event ItemDataChangedEventHandler ItemDataChanged;

        public event DeckDataChangedEventHandler DeckDataChanged;

        static public KCODt Instance {
            get {
                if (instance == null) {
                    instance = new KCODt();
                }
                return instance;
            }
            private set {
                instance = value;
            }
        }

        protected virtual void OnShipSpecChangedEvent (DataChangedEventArgs e) {
            lock (ShipSpecMap) {
                ShipSpec = e.Data;
                ShipSpecMap.Clear();
                foreach (JToken sh in ShipSpec) {
                    ShipSpecMap.Add(sh["api_id"].ToString(), sh);
                }
                ShipSpecChangedEventHandler handler = ShipSpecChanged;
                if (handler != null) {
                    handler(this, e);
                }
            }
        }

        public virtual void OnShipDataChangedEvent (DataChangedEventArgs e) {
            lock (ShipDataMap) {
                ShipData = e.Data;
                ShipDataMap.Clear();
                foreach (JToken sh in ShipData) {
                    ShipDataMap.Add(sh["api_id"].ToString(), sh);
                }
                ShipDataChangedEventHandler handler = ShipDataChanged;
                if (handler != null) {
                    handler(this, e);
                }
            }
        }

        public virtual void OnItemSpecChangedEvent (DataChangedEventArgs e) {
            lock (ItemSpecMap) {
                ItemSpec = e.Data;
                ItemSpecMap.Clear();
                foreach (JToken it in ItemSpec) {
                    ItemSpecMap.Add(it["api_id"].ToString(), it);
                }
                ItemSpecChangedEventHandler handler = ItemSpecChanged;
                if (handler != null) {
                    handler(this, e);
                }
            }
        }

        public virtual void OnItemDataChangedEvent (DataChangedEventArgs e) {
            lock (ItemDataMap) {
                ItemData = e.Data;
                ItemDataMap.Clear();
                foreach (JToken it in ItemData) {
                    ItemDataMap.Add(it["api_id"].ToString(), it);
                }
                ItemDataChangedEventHandler handler = ItemDataChanged;
                if (handler != null) {
                    handler(this, e);
                }
            }
        }

        protected virtual void OnDeckDataChangedEvent (DataChangedEventArgs e) {
            DeckData = e.Data;
            for (int i = 0; i < DeckData.Count(); i++) {
                for (int j = 0; j < DeckData[i]["api_ship"].Count(); j++) {
                    string key = DeckData[i]["api_ship"][j].ToString();
                    if (ShipDataMap.ContainsKey(key)) {
                        JObject jo = ShipDataMap[key] as JObject;
                        if (jo["fleet_info"] == null) {
                            jo.Add("fleet_info", (i + 1) + "-" + (j + 1));
                        }
                    }
                }
            }
            DeckDataChangedEventHandler handler = DeckDataChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        private KCODt () {
            InitializeMasterData();
            testMasterData();
            InitializeFiddler();
        }

        private void InitializeMasterData () {
            ShipSpecMap = new Dictionary<string, JToken>();
            ShipDataMap = new Dictionary<string, JToken>();
            ItemSpecMap = new Dictionary<string, JToken>();
            ItemDataMap = new Dictionary<string, JToken>();
            Assembly assembly = typeof(MainWindow).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.JSON.ship.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                JToken temp = JToken.Parse(json);
                OnShipSpecChangedEvent(new DataChangedEventArgs(temp["api_data"]));
            }
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.JSON.slotitem.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                JToken temp = JToken.Parse(json);
                OnItemSpecChangedEvent(new DataChangedEventArgs(temp["api_data"]));
            }
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.JSON.shiptype.json"))
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
            string pattern = @".*\/kcsapi\/(.*)\/(.*)";
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
                switch (m.Groups[2].ToString()) {
                    case "ship":
                        try {
                            if (m.Groups[1].ToString() == "api_get_master") {
                                string svdata = oS.GetResponseBodyAsString();
                                string json = svdata.Substring(7);
                                JToken temp = JToken.Parse(json);
                                OnShipSpecChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            } else {
                                string svdata = oS.GetResponseBodyAsString();
                                string json = svdata.Substring(7);
                                JToken temp = JToken.Parse(json);
                                OnShipDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            }
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
                            OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data_deck"]));
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
                            OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data"]["api_deck_data"]));
                        } catch (Exception exception) {
                            Debug.Print(exception.Message);
                        }
                        break;
                    case "deck_port":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            JToken temp = JToken.Parse(json);
                            OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                        } catch (Exception exception) {
                            Debug.Print("deck_port parse error: " + exception.Message);
                        }
                        break;
                    case "slotitem":
                        try {
                            if (m.Groups[1].ToString() == "api_get_master") {
                                string svdata = oS.GetResponseBodyAsString();
                                string json = svdata.Substring(7);
                                JToken temp = JToken.Parse(json);
                                OnItemSpecChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            } else {
                                string svdata = oS.GetResponseBodyAsString();
                                string json = svdata.Substring(7);
                                JToken temp = JToken.Parse(json);
                                OnItemDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            }
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