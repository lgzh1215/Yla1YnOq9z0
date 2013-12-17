using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace KanColleTool {
    /// <summary>
    /// ItemList.xaml 的互動邏輯
    /// </summary>
    public partial class ItemListPage : Page {

        Thread UIThread;

        public ItemListPage () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
        }

        void KCODt_SlotItemChanged (object sender, DataChangedEventArgs e) {
            reflash();
        }

        private void reflash () {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    ItemGrid.ItemsSource = null;
                    if (KCODt.Instance.ItemData != null && KCODt.Instance.ItemSpec != null) {
                        var qm = from spec in KCODt.Instance.ItemSpec
                                 from item in KCODt.Instance.ItemData
                                 where spec["api_id"].ToString() == item["api_slotitem_id"].ToString()
                                 select JToken.FromObject(new ItemDetail(spec, item));
                        ItemGrid.ItemsSource = qm;
                    }
                } catch (Exception ex) {
                    Debug.Print(ex.ToString());
                }
            }, null);
        }

        private void Page_Loaded (object sender, RoutedEventArgs e) {
            KCODt.Instance.ItemDataChanged += new KCODt.ItemDataChangedEventHandler(KCODt_SlotItemChanged);
            if (KCODt.Instance.ItemData == null || KCODt.Instance.ItemSpec == null) {
                RequestBuilder.Instance.ReLoadSlotItem();
            }
        }

        private void Page_Unloaded (object sender, RoutedEventArgs e) {
            KCODt.Instance.ItemDataChanged -= new KCODt.ItemDataChangedEventHandler(KCODt_SlotItemChanged);
        }

        private void ItemGrid_KeyDown (object sender, KeyEventArgs e) {
            if (e.Key == Key.F5) {
                RequestBuilder.Instance.ReLoadSlotItem();
            }
        }
    }
}
