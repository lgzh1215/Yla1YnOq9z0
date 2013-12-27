using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace KanColleTool {
    /// <summary>
    /// SpecListPage.xaml 的互動邏輯
    /// </summary>
    public partial class SpecListPage : Page {

        Thread UIThread;

        public SpecListPage () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
            KCODt.Instance.ShipSpecChanged += new KCODt.ShipSpecChangedEventHandler(KCODt_ShipSpecChanged);
            KCODt.Instance.ItemSpecChanged += new KCODt.ItemSpecChangedEventHandler(KCODt_ItemSpecChanged);
        }

        ~SpecListPage () {
            KCODt.Instance.ShipSpecChanged -= new KCODt.ShipSpecChangedEventHandler(KCODt_ShipSpecChanged);
        }

        private void KCODt_ShipSpecChanged (object sender, DataChangedEventArgs e) {
            reflash();
        }

        private void KCODt_ItemSpecChanged (object sender, DataChangedEventArgs e) {
            reflash();
        }

        private void KCODt_StartBattle (object sender, BattleEventArgs e) {
            battle();
        }

        private void reflash () {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    IEnumerable<JToken> ships = from spec in KCODt.Instance.ShipSpec
                                                select spec;
                    ShipGrid.ItemsSource = null;
                    ShipGrid.ItemsSource = ships;
                    IEnumerable<JToken> items = from spec in KCODt.Instance.ItemSpec
                                                select spec;
                    ItemGrid.ItemsSource = null;
                    ItemGrid.ItemsSource = items;
                } catch (Exception ex) {
                    Debug.Print(ex.ToString());
                }
            }, null);
        }

        private void battle () {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    BattleGrid.ItemsSource = null;
                    BattleGrid.ItemsSource = KCODt.Instance.BattleQueue;
                } catch (Exception ex) {
                    Debug.Print(ex.ToString());
                }
            }, null);
        }

        private void ShipGrid_KeyDown (object sender, KeyEventArgs e) {
            if (e.Key == Key.F5) {
                RequestBuilder.Instance.ReLoadShipSpec();
            }
        }

        private void Page_Loaded (object sender, RoutedEventArgs e) {
            reflash();
            KCODt.Instance.StartBattle += new KCODt.StartBattleEventHandler(KCODt_StartBattle);
        }

        private void Page_Unloaded (object sender, RoutedEventArgs e) {
            KCODt.Instance.StartBattle -= new KCODt.StartBattleEventHandler(KCODt_StartBattle);
        }

    }
}
