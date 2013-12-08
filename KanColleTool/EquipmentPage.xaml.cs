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
    /// EquipmentPage.xaml 的互動邏輯
    /// </summary>
    public partial class EquipmentPage : Page {

        private static EquipmentPage instance = null;

        Thread UIThread;

        private int shipId = 0;

        public int ShipId {
            get {
                return shipId;
            }
            set {
                this.shipId = value;
                //reflash();
            }
        }

        static public EquipmentPage Instance {
            get {
                if (instance == null) {
                    instance = new EquipmentPage();
                }
                return instance;
            }
            private set {
                instance = value;
            }
        }

        private EquipmentPage () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
            KCODt.Instance.ItemDataChanged += new KCODt.ItemDataChangedEventHandler(KCODt_ItemDataChanged);
            KCODt.Instance.ShipDataChanged += new KCODt.ShipDataChangedEventHandler(KCODt_ItemDataChanged);
        }

        ~EquipmentPage () {
            KCODt.Instance.ItemDataChanged -= new KCODt.ItemDataChangedEventHandler(KCODt_ItemDataChanged);
            KCODt.Instance.ShipDataChanged -= new KCODt.ShipDataChangedEventHandler(KCODt_ItemDataChanged);
        }

        void KCODt_ItemDataChanged (object sender, DataChangedEventArgs e) {
            reflash();
        }

        private void reflash () {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    if (ShipId == 0) {
                        return;
                    }
                    JToken ship = KCODt.Instance.ShipDataMap[ShipId.ToString()];
                    JToken spec = KCODt.Instance.ShipSpecMap[ship["api_ship_id"].ToString()];
                    labShipName.Content = String.Format("({0}) {1}", ship["api_id"].ToString(), spec["api_name"].ToString());

                    foo(0, labEq1);
                    foo(1, labEq2);
                    foo(2, labEq3);
                    foo(3, labEq4);
                    foo(4, labEq5);
                } catch (Exception ex) {
                    Debug.Print(ex.ToString());
                }
            }, null);
        }

        private void foo (int i, Label lab) {
            JToken ship = KCODt.Instance.ShipDataMap[ShipId.ToString()];
            JToken spec = KCODt.Instance.ShipSpecMap[ship["api_ship_id"].ToString()];
            string eqid = ship["api_slot"][i].ToString();
            string eqName = "無";
            if (eqid != "-1") {
                eqName = KCODt.Instance.ItemDataMap[eqid]["api_name"].ToString();
            }
            lab.Content = String.Format("({0}) {1}", ship["api_onslot"][i].ToString(), eqName);
        }

        private void Page_Loaded (object sender, RoutedEventArgs e) {
            reflash();
        }
    }
}
