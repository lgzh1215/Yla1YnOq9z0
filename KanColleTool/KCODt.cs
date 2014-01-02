using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Fiddler;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace KanColleTool {

    public class KCODt {

        private static KCODt instance = null;

        public bool IsInScenario { get; private set; }
        public JToken ShipSpec { get; private set; }
        public JToken ShipData { get; private set; }
        public JToken ItemSpec { get; private set; }
        public JToken ItemData { get; private set; }
        public JToken DeckData { get; private set; }
        public JToken ShipType { get; private set; }
        public JToken QuestData { get; private set; }
        public JToken NDockData { get; private set; }
        public Dictionary<string, JToken> ShipSpecMap { get; private set; }
        public Dictionary<string, JToken> ShipDataMap { get; private set; }
        public Dictionary<string, JToken> ItemDataMap { get; private set; }
        public Dictionary<string, JToken> ItemSpecMap { get; private set; }
        public Dictionary<string, string> NavalFleet { get; private set; }
        public Dictionary<int, JToken> QuestDataMap { get; private set; }
        public Stack<NavigateDetail> NavigateStack { get; private set; }
        public Dictionary<string, JToken> EnemyDeckMap { get; private set; }

        public delegate void ShipSpecChangedEventHandler (object sender, DataChangedEventArgs e);
        public delegate void ShipDataChangedEventHandler (object sender, DataChangedEventArgs e);
        public delegate void ItemSpecChangedEventHandler (object sender, DataChangedEventArgs e);
        public delegate void ItemDataChangedEventHandler (object sender, DataChangedEventArgs e);
        public delegate void ShipTypeChangedEventHandler (object sender, DataChangedEventArgs e);
        public delegate void DeckDataChangedEventHandler (object sender, DataChangedEventArgs e);
        public delegate void QuestDataChangedEventHandler (object sender, DataChangedEventArgs e);
        public delegate void NDockDataChangedEventHandler (object sender, DataChangedEventArgs e);
        public delegate void ScenarioEventHandler (object sender, EventArgs e);
        public delegate void MapNavigateEventHandler (object sender, NavigateEventArgs e);
        public delegate void BattleEventHandler (object sender, BattleEventArgs e);
        public event ShipSpecChangedEventHandler ShipSpecChanged;
        public event ShipDataChangedEventHandler ShipDataChanged;
        public event ItemSpecChangedEventHandler ItemSpecChanged;
        public event ItemDataChangedEventHandler ItemDataChanged;
        public event ShipTypeChangedEventHandler ShipTypeChanged;
        public event DeckDataChangedEventHandler DeckDataChanged;
        public event QuestDataChangedEventHandler QuestDataChanged;
        public event NDockDataChangedEventHandler NDockDataChanged;
        public event ScenarioEventHandler ScenarioStart;
        public event ScenarioEventHandler ScenarioFinish;
        public event MapNavigateEventHandler MapNavigate;
        public event BattleEventHandler BattleStart;
        public event BattleEventHandler BattleFinish;

        private string defMasterShip = "masterShip.json";
        private string defMasterSlotItem = "masterSlotItem.json";
        private string defMasterSType = "masterSType.json";
        private string defEnemyDeck = "enemyDeck.json";
        private string enemyDeckId = "";
        private string enemyFormation = "";

        #region stadycode
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
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", defMasterShip);
                File.WriteAllText(path, e.Data.ToString());
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
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", defMasterSlotItem);
                File.WriteAllText(path, e.Data.ToString());
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

        public virtual void OnShipTypeChangedEvent (DataChangedEventArgs e) {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", defMasterSType);
            File.WriteAllText(path, e.Data.ToString());
            ShipType = e.Data;
            ShipTypeChangedEventHandler handler = ShipTypeChanged;
            if (handler != null) {
                handler(this, e);
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
        #endregion

        protected virtual void OnScenarioStartEvent (EventArgs e) {
            IsInScenario = true;
            ScenarioEventHandler handler = ScenarioStart;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnScenarioFinishEvent (EventArgs e) {
            IsInScenario = false;
            ScenarioEventHandler handler = ScenarioFinish;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnMapNavigateEvent (NavigateEventArgs e) {
            NavigateStack.Push(new NavigateDetail(e.Type, e.Data));
            if (e.Data["api_enemy"] != null) {
                string eid = e.Data["api_enemy"]["api_enemy_id"].ToString();
                if (!EnemyDeckMap.ContainsKey(eid)) {
                    Debug.Print("EnemyDeckMap doesn't had data on " + eid);
                    enemyDeckId = eid;
                }
            }
            MapNavigateEventHandler handler = MapNavigate;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnBattleStartEvent (BattleEventArgs e) {
            if (enemyDeckId != "" && e.Type == "day") {
                if (e.Data["api_formation"] != null) {
                    enemyFormation = e.Data["api_formation"][1].ToString();
                }
            }
            BattleEventHandler handler = BattleStart;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnBattleFinishEvent (BattleEventArgs e) {
            if (enemyDeckId != "") {
                EnemyDeckInfo info = new EnemyDeckInfo(enemyDeckId, enemyFormation,
                    e.Data["api_enemy_info"]["api_deck_name"].ToString(), e.Data["api_ship_id"]);
                EnemyDeckMap.Add(enemyDeckId, JToken.FromObject(info));
                enemyDeckId = "";
                enemyFormation = "";
                List<string> lx = new List<string>();
                foreach (JToken item in EnemyDeckMap.Values) {
                    lx.Add(item.ToString());
                }
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", defEnemyDeck);
                File.WriteAllText(path, string.Format("[{0}]", string.Join(",", lx)));
            }
            BattleEventHandler handler = BattleFinish;
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
            QuestDataMap = new Dictionary<int, JToken>();
            NavalFleet = new Dictionary<string, string>();
            NavigateStack = new Stack<NavigateDetail>();
            EnemyDeckMap = new Dictionary<string, JToken>();
            Assembly assembly = typeof(MainWindow).Assembly;
            string path;
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", defMasterShip);
            if (File.Exists(path)) {
                string json = File.ReadAllText(path);
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
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", defMasterSlotItem);
            if (File.Exists(path)) {
                string json = File.ReadAllText(path);
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
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", defMasterSType);
            if (File.Exists(path)) {
                string json = File.ReadAllText(path);
                JToken temp = JToken.Parse(json);
                ShipType = JToken.Parse(json);
            } else {
                using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.JSON.masterSType.json"))
                using (StreamReader reader = new StreamReader(stream)) {
                    string json = reader.ReadToEnd();
                    JToken temp = JToken.Parse(json);
                    OnShipTypeChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                }
            }
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", defEnemyDeck);
            if (File.Exists(path)) {
                string json = File.ReadAllText(path);
                JArray temp = JArray.Parse(json);
                foreach (JToken item in temp) {
                    string id = item["Id"].ToString();
                    EnemyDeckMap.Add(id, item);
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
                try {
                    string svdata;
                    string json;
                    JToken temp;
                    switch (m.Groups[2].ToString()) {
                        case "ship":
                            if (m.Groups[1].ToString() == "api_get_master") {
                                svdata = oS.GetResponseBodyAsString();
                                json = svdata.Substring(7);
                                temp = JToken.Parse(json);
                                OnShipSpecChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            } else {
                                svdata = oS.GetResponseBodyAsString();
                                json = svdata.Substring(7);
                                temp = JToken.Parse(json);
                                OnShipDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            }
                            break;
                        case "ship2":
                            svdata = oS.GetResponseBodyAsString();
                            json = svdata.Substring(7);
                            temp = JToken.Parse(json);
                            OnShipDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data_deck"]));
                            break;
                        case "ship3":
                            svdata = oS.GetResponseBodyAsString();
                            json = svdata.Substring(7);
                            temp = JToken.Parse(json);
                            OnShipDataChangedEvent(new DataChangedEventArgs(temp["api_data"]["api_ship_data"]));
                            OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data"]["api_deck_data"]));
                            //OnSlotTypeChangedEvent(new DataChangedEventArgs(temp["api_data"]["api_slot_data"]));
                            break;
                        case "deck_port":
                            svdata = oS.GetResponseBodyAsString();
                            json = svdata.Substring(7);
                            temp = JToken.Parse(json);
                            OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            break;
                        case "deck":
                            svdata = oS.GetResponseBodyAsString();
                            json = svdata.Substring(7);
                            temp = JToken.Parse(json);
                            OnDeckDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            break;
                        case "slotitem":
                            if (m.Groups[1].ToString() == "api_get_master") {
                                svdata = oS.GetResponseBodyAsString();
                                json = svdata.Substring(7);
                                temp = JToken.Parse(json);
                                OnItemSpecChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            } else {
                                svdata = oS.GetResponseBodyAsString();
                                json = svdata.Substring(7);
                                temp = JToken.Parse(json);
                                OnItemDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            }
                            break;
                        case "questlist":
                            svdata = oS.GetResponseBodyAsString();
                            json = svdata.Substring(7);
                            temp = JToken.Parse(json);
                            OnQuestDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            break;
                        case "stype":
                            svdata = oS.GetResponseBodyAsString();
                            json = svdata.Substring(7);
                            temp = JToken.Parse(json);
                            OnShipTypeChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            break;
                        case "mapcell":
                            OnScenarioStartEvent(null);
                            break;
                        case "logincheck":
                            OnScenarioFinishEvent(null);
                            break;
                        case "next":
                            naviEvent(oS, "next");
                            break;
                        case "start":
                            if (m.Groups[1].ToString() == "api_req_map") {
                                naviEvent(oS, "start");
                            }
                            break;
                        case "battle":
                            if (m.Groups[1].ToString() == "api_req_sortie") {
                                battleEvent(oS, "day");
                            } else if (m.Groups[1].ToString() == "api_req_battle_midnight") {
                                battleEvent(oS, "midnight");
                            }
                            break;
                        case "sp_midnight":
                            battleEvent(oS, "sp_midnight");
                            break;
                        case "battleresult":
                            battleEvent(oS, "end");
                            break;
                        case "ndock":
                            svdata = oS.GetResponseBodyAsString();
                            json = svdata.Substring(7);
                            temp = JToken.Parse(json);
                            OnNDockDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
                            break;
                        default:
                            break;
                    }
                } catch (Exception exception) {
                    Debug.Print(exception.ToString());
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

        private void naviEvent (Fiddler.Session oS, string type) {
            try {
                string svdata = oS.GetResponseBodyAsString();
                string json = svdata.Substring(7);
                JToken temp = JToken.Parse(json);
                OnMapNavigateEvent(new NavigateEventArgs(type, temp["api_data"]));
            } catch (Exception exception) {
                Debug.Print(exception.ToString());
            }
        }

        private void battleEvent (Fiddler.Session oS, string type) {
            try {
                string svdata = oS.GetResponseBodyAsString();
                string json = svdata.Substring(7);
                JToken temp = JToken.Parse(json);
                if (type == "end") {
                    OnBattleFinishEvent(new BattleEventArgs(type, temp["api_data"]));
                } else {
                    OnBattleStartEvent(new BattleEventArgs(type, temp["api_data"]));
                }
            } catch (Exception exception) {
                Debug.Print(exception.ToString());
            }
        }
    }
}
