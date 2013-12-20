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
using System.Windows.Data;
using System.Windows.Media.Imaging;

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
            InitializeMenuPanel();
            KCODt.Instance.ItemDataChanged += new KCODt.ItemDataChangedEventHandler(KCODt_ItemDataChanged);
        }

        ~ShipListPage () {
            KCODt.Instance.ItemDataChanged -= new KCODt.ItemDataChangedEventHandler(KCODt_ItemDataChanged);
        }

        private void InitializeMenuPanel () {
            Panel.Add(eq0);
            Panel.Add(eq1);
            Panel.Add(eq2);
            Panel.Add(eq3);
            Panel.Add(eq4);
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
                int shipId = int.Parse(detail["Ship"]["api_id"].ToString());
                int shipIdx = int.Parse(muPos.Uid);
                int fleet = int.Parse(muFle.Uid);
                RequestBuilder.Instance.HenseiChange(shipId, shipIdx, fleet);
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void ShipGrid_ContextMenuOpening (object sender, ContextMenuEventArgs e) {
            try {
                DataGrid dataGrid = sender as DataGrid;
                JToken shipDetail = dataGrid.CurrentItem as JToken;
                int shipId = int.Parse(shipDetail["Ship"]["api_id"].ToString());
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
                        Panel[i].Icon = String.Format("{0}", onslot);
                        Panel[i].Header = String.Format("{0} \t {1}", eqid, eqName);
                    } else {
                        Panel[i].Header = String.Format("{0}", eqName);
                    }
                    Panel[i].ItemsSource = null;
                    Panel[i].ItemsSource = SubMenuItems;
                }
                miFl1.Header = KCODt.Instance.DeckData[0]["api_name"].ToString();
                miFl2.Header = KCODt.Instance.DeckData[1]["api_name"].ToString();
                miFl3.Header = KCODt.Instance.DeckData[2]["api_name"].ToString();
                miFl4.Header = KCODt.Instance.DeckData[3]["api_name"].ToString();
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void queryItems () {
            var qm = (from spec in KCODt.Instance.ItemSpec
                      from item in KCODt.Instance.ItemData
                      where spec["api_id"].ToString() == item["api_slotitem_id"].ToString()
                      select JToken.FromObject(new ItemDetail(spec, item)))
                     .OrderByDescending(x => x["Spec"]["api_rare"].ToString());
            SubMenuItems = qm;
        }

        private void SlotSet_Click (object sender, RoutedEventArgs e) {
            try {
                MenuItem item = e.OriginalSource as MenuItem;
                JToken itemDetail = item.Header as JToken;
                int itemId = int.Parse(itemDetail["Item"]["api_id"].ToString());

                MenuItem muPos = sender as MenuItem;
                ContextMenu contextMenu = muPos.Parent as ContextMenu;
                DataGrid dataGrid = contextMenu.PlacementTarget as DataGrid;
                JToken shipDetail = dataGrid.CurrentItem as JToken;
                int shipId = int.Parse(shipDetail["Ship"]["api_id"].ToString());
                int slotIdx = int.Parse(muPos.Uid);
                RequestBuilder.Instance.SlotSet(slotIdx, itemId, shipId);
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void NDocking_click (object sender, RoutedEventArgs e) {
            try {
                MenuItem item = e.OriginalSource as MenuItem;
                ContextMenu contextMenu = item.Parent as ContextMenu;
                DataGrid dataGrid = contextMenu.PlacementTarget as DataGrid;
                JToken shipDetail = dataGrid.CurrentItem as JToken;
                string shipId = shipDetail["Ship"]["api_id"].ToString();
                KCODt.Instance.FixList.Add(shipId);
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void Destroy_click (object sender, RoutedEventArgs e) {
            try {
                HashSet<string> destroyShips = new HashSet<string>();
                foreach (var item in ShipGrid.SelectedItems) {
                    JToken shipDetail = item as JToken;
                    destroyShips.Add(shipDetail["Ship"]["api_id"].ToString());
                }
                MessageBoxResult messageBoxResult = MessageBox.Show("解體確認", "Delete Confirmation", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes) {
                    RequestBuilder.Instance.DestroyShip(destroyShips);
                }
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void Page_Loaded (object sender, RoutedEventArgs e) {
            KCODt.Instance.ShipDataChanged += new KCODt.ShipDataChangedEventHandler(KCODt_ShipDataChanged);
            KCODt.Instance.DeckDataChanged += new KCODt.DeckDataChangedEventHandler(KCODt_ShipDataChanged);
            if (KCODt.Instance.ShipData == null) {
                RequestBuilder.Instance.ReLoadShip3();
            } else {
                reflash();
            }
        }

        private void Page_Unloaded (object sender, RoutedEventArgs e) {
            KCODt.Instance.ShipDataChanged -= new KCODt.ShipDataChangedEventHandler(KCODt_ShipDataChanged);
            KCODt.Instance.DeckDataChanged -= new KCODt.DeckDataChangedEventHandler(KCODt_ShipDataChanged);
        }

        private void ShipGrid_KeyDown (object sender, KeyEventArgs e) {
            if (e.Key == Key.F5) {
                RequestBuilder.Instance.ReLoadShip3();
            }
        }

    }

    #region converter
    public class UriToImageConverter : IValueConverter {
        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null) {
                return null;
            }
            if (value is string) {
                value = new Uri((string) value);
            }
            if (value is Uri) {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.DecodePixelWidth = 150;
                bi.UriSource = (Uri) value;
                bi.EndInit();
                CroppedBitmap cb = new CroppedBitmap(bi, new Int32Rect(0, 50, 150, 60));
                return cb;
            }
            return null;
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    #endregion
}
