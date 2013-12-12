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

        List<MenuItem> ItemSubMenu = new List<MenuItem>();

        public ShipListPage () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
            KCODt.Instance.ShipDataChanged += new KCODt.ShipDataChangedEventHandler(KCODt_ShipDataChanged);
            KCODt.Instance.DeckDataChanged += new KCODt.DeckDataChangedEventHandler(KCODt_ShipDataChanged);
            KCODt.Instance.ItemDataChanged += new KCODt.ItemDataChangedEventHandler(KCODt_ItemDataChanged);
            Panel.Add(eq0);
            Panel.Add(eq1);
            Panel.Add(eq2);
            Panel.Add(eq3);
            Panel.Add(eq4);
            Panel.Add(eq5);
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
                        eqName = KCODt.Instance.ItemDataMap[eqid]["api_name"].ToString();
                        Panel[i].Icon = String.Format("({0})", onslot);
                        Panel[i].Header = String.Format("{0} \t {1}", eqid, eqName);
                    } else {
                        // put sub menu here
                        Panel[i].Header = String.Format("{0}", eqName);
                        //Panel[i].ItemsSource = ItemSubMenu;
                    }
                }
                Panel[5].ItemsSource = null;
                //Panel[5].Header = "code behide header";
                Panel[5].ItemsSource = ItemSubMenu;
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
            ItemSubMenu.Clear();
            foreach (var item in qm) {
                MenuItem mi = new MenuItem();
                mi.Header = item["Spec"]["api_name"].ToString();
                mi.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                mi.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                ItemSubMenu.Add(mi);
            }
        }
    }

    //class JTokenShipConverter : IValueConverter {
    //    public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
    //        try {
    //            if (value != null) {
    //                JToken sd = value as JToken;
    //                return sd["Spec"]["api_name"].ToString();
    //            }
    //            string empty = "";
    //            return empty;
    //        } catch (Exception) {
    //            return "";
    //        }
    //    }

    //    public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
    //        return null;
    //    }
    //}

    //class JTokenShipConverter2 : IValueConverter {
    //    public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
    //        try {
    //            if (value != null) {
    //                JToken sd = value as JToken;
    //                return sd["Ship"]["api_id"].ToString();
    //            }
    //            string empty = "";
    //            return empty;
    //        } catch (Exception) {
    //            return "";
    //        }
    //    }

    //    public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
    //        return null;
    //    }
    //}

}
