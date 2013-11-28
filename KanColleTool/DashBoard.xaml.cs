using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace KanColleTool {
    /// <summary>
    /// DashBoard.xaml 的互動邏輯
    /// </summary>
    public partial class DashBoard : Page {

        Thread UIThread;
        Timer UITimer;

        public delegate void Invoker ();

        public DashBoard () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
            InitializeTimer();
        }

        void InitializeTimer () {
            TimerCallback tcb = this.update;
            UITimer = new Timer(tcb, null, 0, 1000);
        }

        public void update (Object context) {
            Dispatcher.FromThread(UIThread).Invoke((DashBoard.Invoker) delegate {
                try {
                    JToken jdeck = null;
                    if (KCODt.Ship2 != null) {
                        jdeck = KCODt.Ship2["api_data_deck"];
                    } else if (KCODt.DeckPort != null) {
                        jdeck = KCODt.DeckPort["api_data"];
                    }
                    if (jdeck == null) {
                        return;
                    }
                    labFl1Name.Content = jdeck[0]["api_name"].ToString();
                    // 2
                    labFl2Name.Content = jdeck[1]["api_name"].ToString();
                    labFl2MissionETA.Content = Utils.valueOfUTC(jdeck[1]["api_mission"][2].ToString());
                    TimeSpan span2 = Utils.countSpan(jdeck[1]["api_mission"][2].ToString());
                    labFl2MissionCD.Content = span2.ToString(@"hh\:mm\:ss");
                    // 3
                    labFl3Name.Content = jdeck[2]["api_name"].ToString();
                    labFl3MissionETA.Content = Utils.valueOfUTC(jdeck[2]["api_mission"][2].ToString());
                    TimeSpan span3 = Utils.countSpan(jdeck[2]["api_mission"][2].ToString());
                    labFl3MissionCD.Content = span3.ToString(@"hh\:mm\:ss");
                    // 4
                    labFl4Name.Content = jdeck[3]["api_name"].ToString();
                    labFl4MissionETA.Content = Utils.valueOfUTC(jdeck[3]["api_mission"][2].ToString());
                    TimeSpan span4 = Utils.countSpan(jdeck[3]["api_mission"][2].ToString());
                    labFl4MissionCD.Content = span4.ToString(@"hh\:mm\:ss");
                    if (!RequestBuilder.OnInvoke) {
                        //button1.IsEnabled = true;
                    }
                } catch (Exception e) {
                    Debug.Print(e.ToString());
                }
            }, null);
        }

        private void btnFl1Kira_Click (object sender, RoutedEventArgs e) {
        }

        private void btnFl2Result_Click (object sender, RoutedEventArgs e) {
            try {
                RequestBuilder.MissionReturn(2);
            } catch (Exception ex) {
                Debug.Print(ex.Message);
            }
        }

        private void btnFl3Result_Click (object sender, RoutedEventArgs e) {
            try {
                RequestBuilder.MissionReturn(3);
            } catch (Exception ex) {
                Debug.Print(ex.Message);
            }
        }

        private void btnFl4Result_Click (object sender, RoutedEventArgs e) {
            try {
                RequestBuilder.MissionReturn(4);
            } catch (Exception ex) {
                Debug.Print(ex.Message);
            }
        }

        //private void button1_Click (object sender, RoutedEventArgs e) {
        //    try {
        //        for (int i = 0; i < 4; i++) {
        //            ICollection<string> chargeIds = listChargeShips(i);
        //            RequestBuilder.FleetCharge(i, chargeIds);
        //        }
        //    } catch (Exception ex) {
        //        Debug.Print(ex.Message);
        //    }
        //}

        private ICollection<string> listChargeShips (int fleet) {
            List<JToken> shipIds = KCODt.Ship2["api_data_deck"][fleet]["api_ship"].ToList();
            List<string> tgtShipIds = new List<string>();
            HashSet<string> chargeIds = new HashSet<string>();
            try {
                foreach (var shipId in shipIds) {
                    if (shipId.ToString() != "-1") {
                        tgtShipIds.Add(shipId.ToString());
                    }
                }
                var qs = from sh in KCODt.Ship["api_data"]
                         from s2 in KCODt.Ship2["api_data"]
                         where
                             tgtShipIds.Contains(s2["api_id"].ToString()) &&
                             sh["api_id"].ToString() == s2["api_ship_id"].ToString()
                         select sh;
                foreach (var sid in tgtShipIds) {
                    if (!KCODt.Ship2Map.ContainsKey(sid)) {
                        continue;
                    }
                    JToken myShip = KCODt.Ship2Map[sid];
                    JToken defShip = KCODt.ShipMap[myShip["api_ship_id"].ToString()];
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
}
