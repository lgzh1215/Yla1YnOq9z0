using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using System.Windows.Documents;
using System.Windows.Data;
using System.Windows.Navigation;

namespace KanColleTool {
    /// <summary>
    /// ShipListPage.xaml 的互動邏輯
    /// </summary>
    public partial class ShipListPage : Page {

        Thread UIThread;

        List<MenuItem> Panel = new List<MenuItem>();

        IEnumerable<JToken> SubMenuItems;

        public ShipListPage () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
            KCODt.Instance.ShipDataChanged += new KCODt.ShipDataChangedEventHandler(KCODt_ShipDataChanged);
            KCODt.Instance.DeckDataChanged += new KCODt.DeckDataChangedEventHandler(KCODt_ShipDataChanged);
            KCODt.Instance.ItemDataChanged += new KCODt.ItemDataChangedEventHandler(KCODt_ItemDataChanged);
            InitializeMenuPanel();
        }

        private void InitializeMenuPanel () {
            Panel.Add(eq0);
            Panel.Add(eq1);
            Panel.Add(eq2);
            Panel.Add(eq3);
            Panel.Add(eq4);
        }

        ~ShipListPage () {
            KCODt.Instance.ShipDataChanged -= new KCODt.ShipDataChangedEventHandler(KCODt_ShipDataChanged);
            KCODt.Instance.DeckDataChanged -= new KCODt.DeckDataChangedEventHandler(KCODt_ShipDataChanged);
            KCODt.Instance.ItemDataChanged -= new KCODt.ItemDataChangedEventHandler(KCODt_ItemDataChanged);
        }

        void KCODt_ShipDataChanged (object sender, DataChangedEventArgs e) {
            reflash();
        }

        void KCODt_ItemDataChanged (object sender, DataChangedEventArgs e) {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    queryItems();
                } catch (Exception ex) {
                    Debug.Print(ex.ToString());
                }
            }, null);
        }

        private void reflash () {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    ShipGrid.ItemsSource = null;
                    if (KCODt.Instance.ShipData != null && KCODt.Instance.ShipSpec != null) {
                        var qm = (from spec in KCODt.Instance.ShipSpec
                                  from ship in KCODt.Instance.ShipData
                                  from stype in KCODt.Instance.ShipType
                                  where spec["api_id"].ToString() == ship["api_ship_id"].ToString()
                                  && spec["api_stype"].ToString() == stype["api_id"].ToString()
                                  select JToken.FromObject(new ShipDetail(spec, ship, stype)))
                                 .OrderBy(x => x["FleetInfo"].ToString());
                        ShipGrid.ItemsSource = qm;
                    }
                } catch (Exception ex) {
                    Debug.Print(ex.ToString());
                }
            }, null);
        }

        private void HenseiItem_Click (object sender, RoutedEventArgs e) {
            try {
                MenuItem muPos = sender as MenuItem;
                MenuItem muFle = muPos.Parent as MenuItem;
                MenuItem muExe = muFle.Parent as MenuItem;
                ContextMenu contextMenu = muExe.Parent as ContextMenu;
                DataGrid dataGrid = contextMenu.PlacementTarget as DataGrid;
                JToken detail = dataGrid.CurrentItem as JToken;
                int shipId = Int16.Parse(detail["Ship"]["api_id"].ToString());
                int shipIdx = Int16.Parse(muPos.Uid);
                int fleet = Int16.Parse(muFle.Uid);
                RequestBuilder.Instance.HenseiChange(shipId, shipIdx, fleet);
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void ShipGrid_ContextMenuOpening (object sender, ContextMenuEventArgs e) {
            try {
                DataGrid dataGrid = sender as DataGrid;
                JToken shipDetail = dataGrid.CurrentItem as JToken;
                int shipId = Int32.Parse(shipDetail["Ship"]["api_id"].ToString());
                for (int i = 0; i < 5; i++) {
                    string onslot = shipDetail["Ship"]["api_onslot"][i].ToString();
                    string eqid = shipDetail["Ship"]["api_slot"][i].ToString();
                    string eqName = "無";
                    if (eqid != "-1") {
                        if (KCODt.Instance.ItemDataMap.ContainsKey(eqid)) {
                            eqName = KCODt.Instance.ItemDataMap[eqid]["api_name"].ToString();
                        } else {
                            eqName = "未讀入";
                        }
                        Panel[i].Icon = String.Format("({0})", onslot);
                        Panel[i].Header = String.Format("{0} \t {1}", eqid, eqName);
                    } else {
                        Panel[i].Header = String.Format("{0}", eqName);
                        Panel[i].ItemsSource = null;
                        Panel[i].ItemsSource = SubMenuItems;
                    }
                }
                miFl1.Header = KCODt.Instance.DeckData[0]["api_name"].ToString();
                miFl2.Header = KCODt.Instance.DeckData[1]["api_name"].ToString();
                miFl3.Header = KCODt.Instance.DeckData[2]["api_name"].ToString();
                miFl4.Header = KCODt.Instance.DeckData[3]["api_name"].ToString();
                // TEST
                eqa.ItemsSource = null;
                eqa.ItemsSource = KCODt.Instance.SlotTypeMap.Keys;
                MenuItem sx = eqa.Items[0] as MenuItem;
                sx.ItemsSource = KCODt.Instance.SlotTypeMap[1];
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void queryItems () {
            var qm = (from spec in KCODt.Instance.ItemSpec
                      from item in KCODt.Instance.ItemData
                      where spec["api_id"].ToString() == item["api_slotitem_id"].ToString()
                      select JToken.FromObject(new ItemDetail(spec, item)))
                     .OrderByDescending(x => x["Spec"]["api_rare"].ToString())
                     .Take(10);
            SubMenuItems = qm;
        }

        private void eq0_Click (object sender, RoutedEventArgs e) {
            Debug.Print("Haha");
        }

    }
}
