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

        public DashBoard () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
            InitializeTimer();
            //KCODt.Instance.DeckDataChanged += new KCODt.DeckDataChangedEventHandler(KCODt_DeckDataChanged);
        }

        //~DashBoard() {
        //    KCODt.Instance.DeckDataChanged -= new KCODt.DeckDataChangedEventHandler(KCODt_DeckDataChanged);
        //}

        void InitializeTimer () {
            TimerCallback tcb = this.update;
            UITimer = new Timer(tcb, null, 0, 1000);
        }

        public void update (Object context) {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    if (KCODt.Instance.DeckData == null) {
                        return;
                    }
                    labFl1Name.Content = KCODt.Instance.DeckData[0]["api_name"].ToString();
                    // 2
                    labFl2Name.Content = KCODt.Instance.DeckData[1]["api_name"].ToString();
                    labFl2MissionETA.Content = Utils.valueOfUTC(KCODt.Instance.DeckData[1]["api_mission"][2].ToString());
                    TimeSpan span2 = Utils.countSpan(KCODt.Instance.DeckData[1]["api_mission"][2].ToString());
                    labFl2MissionCD.Content = span2.ToString(@"hh\:mm\:ss");
                    // 3
                    labFl3Name.Content = KCODt.Instance.DeckData[2]["api_name"].ToString();
                    labFl3MissionETA.Content = Utils.valueOfUTC(KCODt.Instance.DeckData[2]["api_mission"][2].ToString());
                    TimeSpan span3 = Utils.countSpan(KCODt.Instance.DeckData[2]["api_mission"][2].ToString());
                    labFl3MissionCD.Content = span3.ToString(@"hh\:mm\:ss");
                    // 4
                    labFl4Name.Content = KCODt.Instance.DeckData[3]["api_name"].ToString();
                    labFl4MissionETA.Content = Utils.valueOfUTC(KCODt.Instance.DeckData[3]["api_mission"][2].ToString());
                    TimeSpan span4 = Utils.countSpan(KCODt.Instance.DeckData[3]["api_mission"][2].ToString());
                    labFl4MissionCD.Content = span4.ToString(@"hh\:mm\:ss");
                    if (!RequestBuilder.OnInvoke) {
                        //button1.IsEnabled = true;
                    }
                } catch (Exception e) {
                    Debug.Print(e.ToString());
                }
            }, null);
        }

        //private void KCODt_DeckDataChanged (object sender, DataChangedEventArgs e) {
        //    Debug.Print("!!!!! DashBoard: SHIP3 INCOME !!!!!!");
        //}

        private void btnFl1Kira_Click (object sender, RoutedEventArgs e) {
        }

        private void btnFl2Result_Click (object sender, RoutedEventArgs e) {
            try {
                RequestBuilder.Instance.MissionReturn(2);
            } catch (Exception ex) {
                Debug.Print(ex.Message);
            }
        }

        private void btnFl3Result_Click (object sender, RoutedEventArgs e) {
            try {
                RequestBuilder.Instance.MissionReturn(3);
            } catch (Exception ex) {
                Debug.Print(ex.Message);
            }
        }

        private void btnFl4Result_Click (object sender, RoutedEventArgs e) {
            try {
                RequestBuilder.Instance.MissionReturn(4);
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

        //private ICollection<string> listChargeShips (int fleet) {
        //    List<JToken> shipIds = KCODt.Instance.DeckData[fleet]["api_ship"].ToList();
        //    List<string> tgtShipIds = new List<string>();
        //    HashSet<string> chargeIds = new HashSet<string>();
        //    try {
        //        foreach (var shipId in shipIds) {
        //            if (shipId.ToString() != "-1") {
        //                tgtShipIds.Add(shipId.ToString());
        //            }
        //        }
        //        var qs = from spec in KCODt.Instance.ShipSpec
        //                 from s2 in KCODt.Instance.ShipData
        //                 where
        //                     tgtShipIds.Contains(s2["api_id"].ToString()) &&
        //                     spec["api_id"].ToString() == s2["api_ship_id"].ToString()
        //                 select spec;
        //        foreach (var sid in tgtShipIds) {
        //            if (!KCODt.Instance.ShipDataMap.ContainsKey(sid)) {
        //                continue;
        //            }
        //            JToken myShip = KCODt.Instance.ShipDataMap[sid];
        //            JToken defShip = KCODt.Instance.ShipSpecMap[myShip["api_ship_id"].ToString()];
        //            string msg = defShip["api_name"].ToString();
        //            msg += "\t\tF: " + myShip["api_fuel"].ToString() + "/" + defShip["api_fuel_max"].ToString();
        //            if (myShip["api_fuel"].ToString() != defShip["api_fuel_max"].ToString()) {
        //                chargeIds.Add(sid);
        //                msg += "*";
        //            }
        //            msg += "\tB: " + myShip["api_bull"].ToString() + "/" + defShip["api_bull_max"].ToString();
        //            if (myShip["api_bull"].ToString() != defShip["api_bull_max"].ToString()) {
        //                chargeIds.Add(sid);
        //                msg += "x";
        //            }
        //            Debug.Print(msg);
        //        }
        //    } catch (Exception ex) {
        //        Debug.Print(ex.Message);
        //    }
        //    return chargeIds;
        //}

    }
}
