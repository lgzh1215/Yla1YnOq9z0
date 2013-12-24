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
        }

        ~SpecListPage () {
            KCODt.Instance.ShipSpecChanged -= new KCODt.ShipSpecChangedEventHandler(KCODt_ShipSpecChanged);
        }

        void KCODt_ShipSpecChanged (object sender, DataChangedEventArgs e) {
            reflash();
        }

        private void reflash () {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    IEnumerable<JToken> qm = from spec in KCODt.Instance.ShipSpec
                                             select spec;
                    ShipGrid.ItemsSource = null;
                    ShipGrid.ItemsSource = qm;
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
            if (ShipGrid.ItemsSource == null) {
                //RequestBuilder.Instance.ReLoadShipSpec();
                reflash();
            } else {
                reflash();
            }
        }

    }
}
