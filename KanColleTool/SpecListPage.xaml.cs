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
using System.IO;

namespace KanColleTool {
    /// <summary>
    /// SpecListPage.xaml 的互動邏輯
    /// </summary>
    public partial class SpecListPage : Page {

        Thread UIThread;

        bool isNextEnemy;

        string eid;

        //JToken eDeck;

        static Dictionary<string, JToken> EDecks = new Dictionary<string, JToken>();
        
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

        private void KCODt_MapNavigation (object sender, NavigateEventArgs e) {
            if (e.Data.Json["api_enemy"] != null) {
                isNextEnemy = true;
                eid = e.Data.Json["api_enemy"]["api_enemy_id"].ToString();
            }
            navigate();
        }

        private void KCODt_Battle (object sender, NavigateEventArgs e) {
            try {
                if (isNextEnemy) {
                    isNextEnemy = false;
                    JToken eDeck = e.Data.Json["api_ship_ke"];
                }
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
            
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

        private void navigate () {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    NaviGrid.ItemsSource = null;
                    NaviGrid.ItemsSource = KCODt.Instance.NavigationQueue;
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
            KCODt.Instance.MapNavigate += new KCODt.MapNavigateEventHandler(KCODt_MapNavigation);
            KCODt.Instance.Battle += new KCODt.BattleEventHandler(KCODt_Battle);
            reflash();
            navigate();
        }

        private void Page_Unloaded (object sender, RoutedEventArgs e) {
            KCODt.Instance.MapNavigate -= new KCODt.MapNavigateEventHandler(KCODt_MapNavigation);
            KCODt.Instance.Battle -= new KCODt.BattleEventHandler(KCODt_Battle);
        }

    }
}
