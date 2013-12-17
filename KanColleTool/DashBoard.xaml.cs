using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace KanColleTool {
    /// <summary>
    /// DashBoard.xaml 的互動邏輯
    /// </summary>

    public partial class DashBoard : Page {

        Thread UIThread;

        Timer UITimer;

        List<DashBoardPanel> Panel = new List<DashBoardPanel>();

        public DashBoard () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
            InitializeMission();
            InitializeTimer();
        }

        void InitializeTimer () {
            TimerCallback tcb = this.update;
            UITimer = new Timer(tcb, null, 0, 1000);
        }

        void InitializeMission () {
            List<MissionDetail> missions = new List<MissionDetail>();
            foreach (var mission in MissionDetail.All) {
                cbxFl1Mission.Items.Add(mission);
                cbxFl2Mission.Items.Add(mission);
                cbxFl3Mission.Items.Add(mission);
                cbxFl4Mission.Items.Add(mission);
            }
            Panel.Add(new DashBoardPanel(cbxFl1Mission, labFl1MissionETA, labFl1MissionCD, btnFl1Result));
            Panel.Add(new DashBoardPanel(cbxFl2Mission, labFl2MissionETA, labFl2MissionCD, btnFl2Result));
            Panel.Add(new DashBoardPanel(cbxFl3Mission, labFl3MissionETA, labFl3MissionCD, btnFl3Result));
            Panel.Add(new DashBoardPanel(cbxFl4Mission, labFl4MissionETA, labFl4MissionCD, btnFl4Result));
        }

        public void update (Object context) {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    if (KCODt.Instance.DeckData == null) {
                        return;
                    }
                    for (int i = 0; i < 4; i++) {
                        JToken fleet = KCODt.Instance.DeckData[i];
                        JToken mission = fleet["api_mission"];
                        //Panel[i].Name.Content = fleet["api_name"].ToString();
                        Panel[i].ETA.Content = Utils.valueOfUTC(mission[2].ToString());
                        Panel[i].CD.Content = Utils.countSpan(mission[2].ToString()).ToString(@"hh\:mm\:ss");
                        if (mission[0].ToString() != "0") {
                            Panel[i].Mission.SelectedIndex = MissionDetail.IdMap[int.Parse(mission[1].ToString())];
                            Panel[i].Mission.IsEnabled = false;
                            Panel[i].Button.IsEnabled = false;
                        } else {
                            Panel[i].Mission.IsEnabled = true;
                            Panel[i].Button.IsEnabled = true;
                            Panel[i].CD.Content = "";
                        }
                    }
                    labShipCount.Content = String.Format("艦娘數 {0}", KCODt.Instance.ShipDataMap.Count);
                    labItemCount.Content = String.Format("裝備數 {0}", KCODt.Instance.ItemDataMap.Count);
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
                // post mission start if possable
                MissionDetail md = (MissionDetail) Panel[pId].Mission.SelectedItem;
                if (md != null && md.Id != 0) {
                    RequestBuilder.Instance.StartMission(pId + 1, md.Id);
                }
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void cbxMission_KeyDown (object sender, KeyEventArgs e) {
            if (e.Key == Key.Back) {
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

    }

    class DashBoardPanel {
        public ComboBox Mission { get; private set; }
        public Label ETA { get; private set; }
        public Label CD { get; private set; }
        public Button Button { get; private set; }

        public DashBoardPanel (ComboBox mission, Label eta, Label cd, Button button) {
            Mission = mission;
            ETA = eta;
            CD = cd;
            Button = button;
        }
    }

}
