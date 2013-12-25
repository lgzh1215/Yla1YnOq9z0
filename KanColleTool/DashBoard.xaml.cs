using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace KanColleTool {
    /// <summary>
    /// DashBoard.xaml 的互動邏輯
    /// </summary>

    public partial class DashBoard : Page {

        Thread UIThread;

        List<Timer> NDTimers = new List<Timer>();

        List<NDockPanel> NDPanels = new List<NDockPanel>();

        Timer UITimer;

        List<DashBoardPanel> Panel = new List<DashBoardPanel>();

        UriToImageConverter uic = new UriToImageConverter();

        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        BitmapImage eng0;
        BitmapImage eng1;
        BitmapImage eng2;
        BitmapImage eng3;

        public DashBoard () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
            InitializeMission();
            InitializeTimer();
            KCODt.Instance.NDockDataChanged += new KCODt.NDockDataChangedEventHandler(KCODt_NDockDataChanged);
            KCODt.Instance.FixList.CollectionChanged += new NotifyCollectionChangedEventHandler(FixList_CollectionChanged);
        }

        private void FixList_CollectionChanged (object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                Debug.Print(sender.ToString());
                for (int i = 0; i < KCODt.Instance.FixList.Count; i++) {
                    string shipId = KCODt.Instance.FixList[i];
                    if (!KCODt.Instance.ShipDataMap.ContainsKey(shipId)) {
                        KCODt.Instance.FixList.RemoveAt(i);
                        continue;
                    }
                    JToken ship = KCODt.Instance.ShipDataMap[shipId];
                    string ndtime = ship["api_ndock_time"].ToString();
                    if (ndtime == "0") {
                        KCODt.Instance.FixList.RemoveAt(i);
                        continue;
                    }
                }
            }
        }

        private void KCODt_NDockDataChanged (object sender, DataChangedEventArgs e) {
            ndocking(e);
        }

        private void InitializeTimer () {
            TimerCallback tcb = this.update;
            UITimer = new Timer(tcb, null, 0, 1000);
            NDTimers.Add(null);
            NDTimers.Add(null);
            NDTimers.Add(null);
            NDTimers.Add(null);
            NDTimers.Add(null);
            NDPanels.Add(new NDockPanel(stpNDock1Panel));
            NDPanels.Add(new NDockPanel(stpNDock2Panel));
        }

        private void InitializeMission () {
            eng0 = (BitmapImage) this.FindResource("eng0");
            eng1 = (BitmapImage) this.FindResource("eng1");
            eng2 = (BitmapImage) this.FindResource("eng2");
            eng3 = (BitmapImage) this.FindResource("eng3");
            Panel.Add(new DashBoardPanel(stpFleet1Panel));
            Panel.Add(new DashBoardPanel(stpFleet2Panel));
            Panel.Add(new DashBoardPanel(stpFleet3Panel));
            Panel.Add(new DashBoardPanel(stpFleet4Panel));
            foreach (DashBoardPanel item in Panel) {
                item.Mission.ItemsSource = MissionDetail.All;
            }
        }

        private void update (Object context) {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    if (KCODt.Instance.DeckData == null) {
                        return;
                    }
                    for (int i = 0; i < Panel.Count; i++) {
                        JToken fleet = KCODt.Instance.DeckData[i];
                        JToken mission = fleet["api_mission"];
                        Panel[i].ETA.Content = Utils.valueOfUTC(mission[2].ToString());
                        TimeSpan cd = Utils.countSpan(mission[2].ToString());
                        Panel[i].CD.Content = String.Format("{0:00}:{1}", (int) cd.TotalHours, cd.ToString(@"mm\:ss"));
                        if (mission[0].ToString() != "0") {
                            Panel[i].Mission.SelectedIndex = MissionDetail.IdMap[int.Parse(mission[1].ToString())];
                            Panel[i].Mission.IsEnabled = false;
                            Panel[i].Button.IsEnabled = false;
                        } else {
                            Panel[i].Mission.IsEnabled = true;
                            Panel[i].Button.IsEnabled = true;
                            Panel[i].CD.Content = "";
                        }
                        // check fuel && cond
                        Panel[i].Fuel.Visibility = System.Windows.Visibility.Hidden;
                        Panel[i].Cond.Visibility = System.Windows.Visibility.Hidden;
                        if (KCODt.Instance.ShipData == null) {
                            continue;
                        }

                        int minCond = 99;
                        foreach (JToken item in fleet["api_ship"]) {
                            string shipId = item.ToString();
                            if (KCODt.Instance.ShipDataMap.ContainsKey(shipId)) {
                                int fc = int.Parse(KCODt.Instance.ShipDataMap[shipId]["api_fuel"].ToString());
                                string specId = KCODt.Instance.ShipDataMap[shipId]["api_ship_id"].ToString();
                                JToken spec = KCODt.Instance.ShipSpecMap[specId];
                                int fs = int.Parse(spec["api_fuel_max"].ToString());
                                if (fs - fc > 0) {
                                    Panel[i].Fuel.Visibility = System.Windows.Visibility.Visible;
                                }
                                int cond = int.Parse(KCODt.Instance.ShipDataMap[shipId]["api_cond"].ToString());
                                minCond = Math.Min(minCond, cond);

                            }
                        }
                        if (minCond > 49) {
                            Panel[i].Cond.Source = eng3;
                            Panel[i].Cond.Visibility = System.Windows.Visibility.Visible;
                        } else if (minCond <= 49 && minCond >= 40) {
                            Panel[i].Cond.Source = eng2;
                            Panel[i].Cond.Visibility = System.Windows.Visibility.Visible;
                        } else if (minCond < 40 && minCond >= 30) {
                            Panel[i].Cond.Source = eng1;
                            Panel[i].Cond.Visibility = System.Windows.Visibility.Visible;
                        } else {
                            Panel[i].Cond.Source = eng0;
                            Panel[i].Cond.Visibility = System.Windows.Visibility.Visible;
                        }
                        Panel[i].CondNo.Content = minCond;
                    }

                    labShipCount.Content = String.Format("艦娘數 {0}", KCODt.Instance.ShipDataMap.Count);
                    labItemCount.Content = String.Format("裝備數 {0}", KCODt.Instance.ItemDataMap.Count);

                    for (int i = 0; i < NDPanels.Count; i++) {
                        if (NDPanels[i].Visibility == Visibility.Visible) {
                            long x = (long) (DateTime.UtcNow - start).TotalMilliseconds;
                            NDPanels[i].Bar.Value = x;
                            JToken ndock = KCODt.Instance.NDockData[i];
                            string time = ndock["api_complete_time"].ToString();
                            NDPanels[i].ETA.Content = Utils.valueOfUTC(time);
                            NDPanels[i].CD.Content = Utils.countSpan(time).ToString(@"hh\:mm\:ss");
                        }
                    }
                } catch (Exception e) {
                    Debug.Print(e.ToString());
                }
            }, null);
        }

        private void btnFleet_Click (object sender, RoutedEventArgs e) {
            try {
                Button btn = sender as Button;
                int pId = int.Parse(btn.Uid);
                ICollection<string> chargeIds = listChargeShips(pId);
                if (chargeIds.Count > 0) {
                    RequestBuilder.Instance.FleetCharge(pId, chargeIds);
                }
                MissionDetail md = (MissionDetail) Panel[pId].Mission.SelectedItem;
                if (md != null && md.Id != 0) {
                    RequestBuilder.Instance.StartMission(pId + 1, md.Id);
                }
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void cbxMission_KeyDown (object sender, KeyEventArgs e) {
            if (e.Key == Key.Delete) {
                ComboBox cb = sender as ComboBox;
                cb.SelectedItem = null;
            }
        }

        private ICollection<string> listChargeShips (int fleet) {
            List<JToken> shipIds = KCODt.Instance.DeckData[fleet]["api_ship"].ToList();
            List<string> tgtShipIds = new List<string>();
            HashSet<string> chargeIds = new HashSet<string>();
            try {
                foreach (var shipId in shipIds) {
                    if (shipId.ToString() != "-1") {
                        tgtShipIds.Add(shipId.ToString());
                    }
                }
                var qs = from spec in KCODt.Instance.ShipSpec
                         from s2 in KCODt.Instance.ShipData
                         where
                             tgtShipIds.Contains(s2["api_id"].ToString()) &&
                             spec["api_id"].ToString() == s2["api_ship_id"].ToString()
                         select spec;
                foreach (var sid in tgtShipIds) {
                    if (!KCODt.Instance.ShipDataMap.ContainsKey(sid)) {
                        continue;
                    }
                    JToken myShip = KCODt.Instance.ShipDataMap[sid];
                    JToken defShip = KCODt.Instance.ShipSpecMap[myShip["api_ship_id"].ToString()];
                    string msg = defShip["api_name"].ToString();
                    msg += "\t\t\tF: " + myShip["api_fuel"].ToString() + "/" + defShip["api_fuel_max"].ToString();
                    if (myShip["api_fuel"].ToString() != defShip["api_fuel_max"].ToString()) {
                        chargeIds.Add(sid);
                        msg += "*";
                    }
                    msg += "\t\tB: " + myShip["api_bull"].ToString() + "/" + defShip["api_bull_max"].ToString();
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

        private IEnumerable<JToken> availableNDock () {
            IEnumerable<JToken> qm = from ndock in KCODt.Instance.NDockData
                                     where ndock["api_state"].ToString() != "-1"
                                     select ndock;
            return qm;
        }

        private void checkNDock (Object context) {
            try {
                RequestBuilder.Instance.EnterNDock();
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void findNShips (Object context) {
            try {
                var ns = from ndock in KCODt.Instance.NDockData
                         select ndock["api_ship_id"].ToString();
                var qm = (from spec in KCODt.Instance.ShipSpec
                          from ship in KCODt.Instance.ShipData
                          from stype in KCODt.Instance.ShipType
                          where spec["api_id"].ToString() == ship["api_ship_id"].ToString()
                          && spec["api_stype"].ToString() == stype["api_id"].ToString()
                          && ship["api_ndock_time"].ToString() != "0"
                          && !ns.Contains(ship["api_id"].ToString())
                          select JToken.FromObject(new ShipDetail(spec, ship, stype)))
                          .OrderBy(x => long.Parse(x["Ship"]["api_ndock_time"].ToString()))
                          .Take(1);
                foreach (JToken sd in qm) {
                    string api_ndock_id = context as string;
                    int ndockId = int.Parse(api_ndock_id);
                    int shipId = int.Parse(sd["Ship"]["api_id"].ToString());
                    Debug.Print(String.Format("next nd ship is ({0}) {1} enter No.{2} dock", shipId, sd["Spec"]["api_name"], api_ndock_id));
                    Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                        if (chkAutoNDock.IsChecked == true) {
                            RequestBuilder.Instance.NDockStart(shipId, ndockId, 0);
                        }
                    });
                }
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void ndocking (Object context) {
            try {
                if (KCODt.Instance.NDockData == null) {
                    return;
                }
                IEnumerable<JToken> ndocks = availableNDock();
                int count = int.Parse(ndocks.Count().ToString());
                if (KCODt.Instance.ShipSpec == null || KCODt.Instance.ShipData == null || count < 1) {
                    return;
                }
                foreach (JToken ndock in ndocks) {
                    int timerId = int.Parse(ndock["api_id"].ToString());
                    Timer timer;
                    int dueTime = 2000;
                    if (ndock["api_complete_time"].ToString() == "0") {
                        timer = new Timer(this.findNShips, ndock["api_id"].ToString(), dueTime, Timeout.Infinite);
                        Debug.Print(string.Format("find ship after {0} ms", dueTime));
                        closeNDockPanel(timerId - 1);
                        timerId = 0;
                    } else {
                        long x = long.Parse(ndock["api_complete_time"].ToString());
                        x -= 50000;
                        DateTime dt = Utils.parseUTC(x.ToString());
                        TimeSpan ts = dt - DateTime.Now;
                        dueTime = int.Parse(ts.TotalMilliseconds.ToString("0"));
                        if (dueTime < 0) {
                            continue;
                        }
                        timer = new Timer(this.checkNDock, null, dueTime, Timeout.Infinite);
                        Debug.Print(string.Format("nodock {0} finish after {1} ms", ndock["api_id"], ts));
                        setupNDockPanel(timerId - 1, ndock["api_ship_id"].ToString(), ndock);
                    }
                    if (NDTimers[timerId] != null) {
                        NDTimers[timerId].Dispose();
                    }
                    NDTimers[timerId] = timer;
                }

                var qm = (from spec in KCODt.Instance.ShipSpec
                          from ship in KCODt.Instance.ShipData
                          from stype in KCODt.Instance.ShipType
                          where spec["api_id"].ToString() == ship["api_ship_id"].ToString()
                          && spec["api_stype"].ToString() == stype["api_id"].ToString()
                          && ship["api_ndock_time"].ToString() != "0"
                          && !KCODt.Instance.NavalFleet.Keys.Contains(ship["api_id"].ToString())
                          select JToken.FromObject(new ShipDetail(spec, ship, stype)))
                          .OrderBy(x => long.Parse(x["Ship"]["api_ndock_time"].ToString()));
                foreach (var sd in qm) {
                    long t = long.Parse(sd["Ship"]["api_ndock_time"].ToString() + "0000");
                    TimeSpan ts = new TimeSpan(t);
                    Debug.Print(String.Format("({0}) {1} \t\t {2}", sd["Ship"]["api_id"], sd["Spec"]["api_name"], ts));
                }
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void closeNDockPanel (int pid) {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    NDPanels[pid].Visibility = Visibility.Hidden;
                } catch (Exception ex) {
                    Debug.Print(ex.ToString());
                }
            });
        }

        private void setupNDockPanel (int pid, string shipId, JToken ndock) {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    var qs = from spec in KCODt.Instance.ShipSpec
                             from ship in KCODt.Instance.ShipData
                             from stype in KCODt.Instance.ShipType
                             where spec["api_id"].ToString() == ship["api_ship_id"].ToString()
                             && spec["api_stype"].ToString() == stype["api_id"].ToString()
                             && ship["api_id"].ToString() == shipId
                             select JToken.FromObject(new ShipDetail(spec, ship, stype));
                    JToken sd = qs.First() as JToken;
                    NDPanels[pid].Icon.Source = (CroppedBitmap) uic.Convert(new Uri(sd["ShipIcoName"].ToString()),
                        null, null, null);

                    long t = long.Parse(sd["Ship"]["api_ndock_time"].ToString());
                    NDPanels[pid].Bar.Maximum = double.Parse(ndock["api_complete_time"].ToString()) - 60000;
                    NDPanels[pid].Bar.Minimum = NDPanels[pid].Bar.Maximum - t;
                    NDPanels[pid].Visibility = Visibility.Visible;
                    Debug.Print("bar from: {0} -> {1}", NDPanels[pid].Bar.Minimum, NDPanels[pid].Bar.Maximum);
                } catch (Exception ex) {
                    Debug.Print(ex.ToString());
                }
            });
        }

        private void chkAutoNDock_Checked (object sender, RoutedEventArgs e) {
            try {
                CheckBox cbx = sender as CheckBox;
                if (cbx.IsChecked.HasValue && (bool) cbx.IsChecked) {
                    checkNDock(null);
                }
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }
    }

    class DashBoardPanel {
        private int imgCond = 0;
        private int imgFuel = 1;
        private int labCond = 2;
        private int cbxMission = 3;
        private int btnButton = 4;
        private int labCD = 5;
        private int labETA = 6;

        #region getter
        public Image Cond {
            get {
                return stackPanel.Children[imgCond] as Image;
            }
            private set { }
        }

        public Image Fuel {
            get {
                return stackPanel.Children[imgFuel] as Image;
            }
            private set { }
        }

        public ComboBox Mission {
            get {
                return stackPanel.Children[cbxMission] as ComboBox;
            }
            private set { }
        }

        public Label ETA {
            get {
                return stackPanel.Children[labETA] as Label;
            }
            private set { }
        }

        public Label CD {
            get {
                return stackPanel.Children[labCD] as Label;
            }
            private set { }
        }

        public Button Button {
            get {
                return stackPanel.Children[btnButton] as Button;
            }
            private set { }
        }

        public Label CondNo {
            get {
                return stackPanel.Children[labCond] as Label;
            }
            private set { }
        }
        #endregion

        private StackPanel stackPanel;

        public DashBoardPanel (StackPanel stackPanel) {
            this.stackPanel = stackPanel;
        }
    }

    class NDockPanel {
        private int imgIcon = 0;
        private int prgBar = 1;
        private int labCD = 2;
        private int labETA = 3;

        public Image Icon {
            get {
                return stackPanel.Children[imgIcon] as Image;
            }
            private set { }
        }

        public Label ETA {
            get {
                return stackPanel.Children[labETA] as Label;
            }
            private set { }
        }

        public Label CD {
            get {
                return stackPanel.Children[labCD] as Label;
            }
            private set { }
        }

        public ProgressBar Bar {
            get {
                return stackPanel.Children[prgBar] as ProgressBar;
            }
            private set { }
        }

        public Visibility Visibility {
            get {
                return stackPanel.Visibility;
            }
            set {
                stackPanel.Visibility = value;
            }
        }

        private StackPanel stackPanel;

        public NDockPanel (StackPanel stackPanel) {
            this.stackPanel = stackPanel;
            this.CD.VerticalAlignment = VerticalAlignment.Center;
            this.ETA.VerticalAlignment = VerticalAlignment.Center;
            this.Visibility = Visibility.Hidden;
        }
    }
}
