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
    /// ItemList.xaml 的互動邏輯
    /// </summary>
    public partial class ItemListPage : Page {

        Thread UIThread;

        public ItemListPage () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
            KCODt.Instance.ItemDataChanged += new KCODt.ItemDataChangedEventHandler(KCODt_SlotItemChanged);
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
                        //var qm = from spec in KCODt.Instance.ItemSpec
                        //         select JToken.FromObject(new ItemDetail(spec, null));
                        ItemGrid.ItemsSource = qm;
                    }
                } catch (Exception ex) {
                    Debug.Print(ex.Message);
                }
            }, null);
        }
    }
}
