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
    /// ShipListPage.xaml 的互動邏輯
    /// </summary>
    public partial class ShipListPage : Page {

        Thread UIThread;

        public ShipListPage () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
            KCODt.Instance.ShipDataChanged += new KCODt.ShipDataChangedEventHandler(KCODt_ShipDataChanged);
        }

        ~ShipListPage () {
            KCODt.Instance.ShipDataChanged -= new KCODt.ShipDataChangedEventHandler(KCODt_ShipDataChanged);
        }

        void KCODt_ShipDataChanged (object sender, DataChangedEventArgs e) {
            reflash();
        }

        private void reflash () {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    ShipGrid.ItemsSource = null;
                    if (KCODt.Instance.ShipData != null && KCODt.Instance.ShipSpec != null) {
                        var qm = from spec in KCODt.Instance.ShipSpec
                                 from ship in KCODt.Instance.ShipData
                                 from stype in KCODt.Instance.ShipType
                                 where spec["api_id"].ToString() == ship["api_ship_id"].ToString()
                                 && spec["api_stype"].ToString() == stype["api_id"].ToString()
                                 select JToken.FromObject(new ShipDetail(spec, ship, stype));
                        ShipGrid.ItemsSource = qm;
                    }
                } catch (Exception ex) {
                    Debug.Print(ex.Message);
                }
            }, null);
        }

    }

}
