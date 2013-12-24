using System;
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
using System.Collections.ObjectModel;

namespace KanColleTool {

    public class KCODt {

        private static KCODt instance = null;

        private ObservableCollection<string> fixList;
        public ObservableCollection<string> FixList {
            get {
                if (fixList == null) {
                    fixList = new ObservableCollection<string>();
                }
                return fixList;
            }
        }

        public JToken ShipSpec { get; private set; }
        public JToken ShipData { get; private set; }
        public JToken ItemSpec { get; private set; }
        public JToken ItemData { get; private set; }
        public JToken DeckData { get; private set; }
        public JToken ShipType { get; private set; }
        public JToken SlotType { get; private set; }
        public JToken QuestData { get; private set; }
        public JToken NDockData { get; private set; }
        public Dictionary<string, JToken> ShipSpecMap { get; private set; }
        public Dictionary<string, JToken> ShipDataMap { get; private set; }
        public Dictionary<string, JToken> ItemDataMap { get; private set; }
        public Dictionary<string, JToken> ItemSpecMap { get; private set; }
        public Dictionary<string, string> NavalFleet { get; private set; }
        public Dictionary<int, List<int>> SlotTypeMap { get; private set; }
        public Dictionary<int, JToken> QuestDataMap { get; private set; }

        public delegate void ShipSpecChangedEventHandler (object sender, DataChangedEventArgs e);

        public delegate void ShipDataChangedEventHandler (object sender, DataChangedEventArgs e);

        public delegate void ItemSpecChangedEventHandler (object sender, DataChangedEventArgs e);

        public delegate void ItemDataChangedEventHandler (object sender, DataChangedEventArgs e);

        public delegate void SlotTypeChangedEventHandler (object sender, DataChangedEventArgs e);

        public delegate void DeckDataChangedEventHandler (object sender, DataChangedEventArgs e);

        public delegate void QuestDataChangedEventHandler (object sender, DataChangedEventArgs e);

        public delegate void NDockDataChangedEventHandler (object sender, DataChangedEventArgs e);

        public event ShipSpecChangedEventHandler ShipSpecChanged;

        public event ShipDataChangedEventHandler ShipDataChanged;

        public event ItemSpecChangedEventHandler ItemSpecChanged;

        public event ItemDataChangedEventHandler ItemDataChanged;

        public event SlotTypeChangedEventHandler SlotTypeChanged;

        public event DeckDataChangedEventHandler DeckDataChanged;

        public event QuestDataChangedEventHandler QuestDataChanged;

        public event NDockDataChangedEventHandler NDockDataChanged;

        private string defMasterShip = @"D:\usr\KanColleTool\masterShip.json";

        private string defMasterSlotItem = @"D:\usr\KanColleTool\masterSlotItem.json";

        private string defMasterSType = @"D:\usr\KanColleTool\masterSType.json";

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
                File.WriteAllText(defMasterShip, e.Data.ToString());
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
                File.WriteAllText(defMasterSlotItem, e.Data.ToString());
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

        public virtual void OnSlotTypeChangedEvent (DataChangedEventArgs e) {
            string pattern = @"api_slottype(\d*)";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            lock (SlotTypeMap) {
                SlotType = e.Data;
                SlotTypeMap.Clear();
                foreach (JToken element in SlotType) {
                    JProperty prop = element as JProperty;
                    Match m = Regex.Match(prop.Name, pattern);
                    string group = m.Groups[1].ToString();
                    int typeId = int.Parse(group);
                    List<JToken> items = prop.Value.ToList();
                    if (items.Count == 0) {
                        continue;
                    }
                    if (!SlotTypeMap.ContainsKey(typeId)) {
                        SlotTypeMap[typeId] = new List<int>();
                    }
                    foreach (var item in items) {
                        int id = int.Parse(item.ToString());
                        SlotTypeMap[typeId].Add(id);
                    }
                }
                SlotTypeChangedEventHandler handler = SlotTypeChanged;
                if (handler != null) {
                    handler(this, e);
                }
            }
        }

        protected virtual void OnDeckDataChangedEvent (DataChangedEventArgs e) {
            DeckData = e.Data;
            NavalFleet.Clear();
            for (int i = 0; i < DeckData.Count(); i++) {
                for (int j = 0; j < DeckData[i]["api_ship"].Count(); j++) {
                    string key = DeckData[i]["api_ship"][j].ToString();
                    if (ShipDataMap.ContainsKey(key)) {
                        NavalFleet[key] = (i + 1) + "-" + (j + 1);
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

        public virtual void OnQuestDataChangedEvent (DataChangedEventArgs e) {
            lock (QuestDataMap) {
                QuestData = e.Data;
                foreach (JToken it in QuestData["api_list"]) {
                    if (it.ToString() == "-1") {
                        continue;
                    }
                    int questId = int.Parse(it["api_no"].ToString());
                    if (QuestDataMap.ContainsKey(questId)) {
                        QuestDataMap[questId] = it;
                    } else {
                        QuestDataMap.Add(questId, it);
                    }
                }
                QuestDataChangedEventHandler handler = QuestDataChanged;
                if (handler != null) {
                    handler(this, e);
                }
            }
        }

        protected virtual void OnNDockDataChangedEvent (DataChangedEventArgs e) {
            NDockData = e.Data;
            NDockDataChangedEventHandler handler = NDockDataChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        private KCODt () {
            InitializeMasterData();
            InitializeFiddler();
        }

        private void InitializeMasterData () {
            ShipSpecMap = new Dictionary<string, JToken>();
            ShipDataMap = new Dictionary<string, JToken>();
            ItemSpecMap = new Dictionary<string, JToken>();
            ItemDataMap = new Dictionary<string, JToken>();
            SlotTypeMap = new Dictionary<int, List<int>>();
            QuestDataMap = new Dictionary<int, JToken>();
            NavalFleet = new Dictionary<string, string>();
            Assembly assembly = typeof(MainWindow).Assembly;

            if (File.Exists(defMasterShip)) {
                string json = File.ReadAllText(defMasterShip);
                JToken temp = JToken.Parse(json);
                OnShipSpecChangedEvent(new DataChangedEventArgs(temp));
            } else {
                using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.JSON.masterShip.json"))
                using (StreamReader reader = new StreamReader(stream)) {
                    string json = reader.ReadToEnd();
                    JToken temp = JToken.Parse(json);
                    OnShipSpecChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                }
            }
            if (File.Exists(defMasterSlotItem)) {
                string json = File.ReadAllText(defMasterSlotItem);
                JToken temp = JToken.Parse(json);
                OnItemSpecChangedEvent(new DataChangedEventArgs(temp));
            } else {
                using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.JSON.masterSlotItem.json"))
                using (StreamReader reader = new StreamReader(stream)) {
                    string json = reader.ReadToEnd();
                    JToken temp = JToken.Parse(json);
                    OnItemSpecChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                }
            }
            if (File.Exists(defMasterSType)) {
                string json = File.ReadAllText(defMasterSType);
                JToken temp = JToken.Parse(json);
                ShipType = JToken.Parse(json);
            } else {
                using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.JSON.masterSType.json"))
                using (StreamReader reader = new StreamReader(stream)) {
                    string json = reader.ReadToEnd();
                    ShipType = JToken.Parse(json)["api_data"];
                }
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
                            Debug.Print(exception.ToString());
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
                            Debug.Print(exception.ToString());
                        }
                        break;
                    case "ship3":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            JToken temp = JToken.Parse(json);
                            OnShipDataChangedEvent(new DataChangedEventArgs(temp["api_data"]["api_ship_data"]));
                            OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data"]["api_deck_data"]));
                            OnSlotTypeChangedEvent(new DataChangedEventArgs(temp["api_data"]["api_slot_data"]));
                        } catch (Exception exception) {
                            Debug.Print(exception.ToString());
                        }
                        break;
                    case "deck_port":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            JToken temp = JToken.Parse(json);
                            OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                        } catch (Exception exception) {
                            Debug.Print("deck_port parse error: " + exception.ToString());
                        }
                        break;
                    case "deck":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            JToken temp = JToken.Parse(json);
                            OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                        } catch (Exception exception) {
                            Debug.Print("deck parse error: " + exception.ToString());
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
                            Debug.Print(exception.ToString());
                        }
                        break;
                    case "questlist":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            JToken temp = JToken.Parse(json);
                            OnQuestDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                        } catch (Exception exception) {
                            Debug.Print("quest parse error: " + exception.ToString());
                        }
                        break;
                    case "next":
                        printInfo(oS);
                        break;
                    case "start":
                        if (m.Groups[1].ToString() == "api_req_map") {
                            printInfo(oS);
                        } else if (m.Groups[1].ToString() == "api_req_nyukyo") {
                            // TODO
                        }
                        break;
                    case "ndock":
                        try {
                            string svdata = oS.GetResponseBodyAsString();
                            string json = svdata.Substring(7);
                            JToken temp = JToken.Parse(json);
                            OnNDockDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                        } catch (Exception exception) {
                            Debug.Print("quest parse error: " + exception.ToString());
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

        private void printInfo (Fiddler.Session oS) {
            try {
                string svdata = oS.GetResponseBodyAsString();
                string json = svdata.Substring(7);
                JToken temp = JToken.Parse(json);
                Debug.Print(temp.ToString());
            } catch (Exception exception) {
                Debug.Print("next parse error: " + exception.ToString());
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
