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
    /// QuestList.xaml 的互動邏輯
    /// </summary>
    public partial class QuestListPage : Page {

        Thread UIThread;

        public QuestListPage () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
        }

        private void Page_Loaded (object sender, RoutedEventArgs e) {
            reflash();
        }

        private void reflash () {
            try {
                KCODt.Instance.QuestDataMap.Clear();
                RequestBuilder.Instance.QuestList(1);
                KCODt.Instance.QuestDataChanged += new KCODt.QuestDataChangedEventHandler(KCODt_QuestDataChanged);
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        void KCODt_QuestDataChanged (object sender, DataChangedEventArgs e) {
            JToken result = KCODt.Instance.QuestData;
            int count = int.Parse(result["api_count"].ToString());
            int currentPage = int.Parse(result["api_disp_page"].ToString());
            if (currentPage * 5 >= count) {
                KCODt.Instance.QuestDataChanged -= new KCODt.QuestDataChangedEventHandler(KCODt_QuestDataChanged);
                Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                    try {
                        var qm = (from quest in KCODt.Instance.QuestDataMap.Values
                                  select JToken.FromObject(new QuestDetail(quest)));
                        QuestGrid.ItemsSource = null;
                        QuestGrid.ItemsSource = qm;
                    } catch (Exception ex) {
                        Debug.Print(ex.ToString());
                    }
                }, null);
            } else {
                RequestBuilder.Instance.QuestList(currentPage + 1);
            }
        }

        private void QuestGrid_ContextMenuOpening (object sender, ContextMenuEventArgs e) {
            try {
                cm.Items.Clear();
                DataGrid dataGrid = sender as DataGrid;
                JToken questDetail = dataGrid.CurrentItem as JToken;
                string status = questDetail["Status"].ToString();
                string progress = questDetail["Progress"].ToString();
                string id = questDetail["Data"]["api_no"].ToString();
                if ("" == status) {
                    MenuItem questStart = new MenuItem();
                    questStart.Click += new RoutedEventHandler(TakeQuest_Click);
                    QuestAction x = new QuestAction("接受", int.Parse(id), "start");
                    questStart.Header = x;
                    cm.Items.Add(questStart);
                } else if ("執行中" == status) {
                    MenuItem questStop = new MenuItem();
                    questStop.Click += new RoutedEventHandler(TakeQuest_Click);
                    QuestAction x = new QuestAction("取消", int.Parse(id), "stop");
                    questStop.Header = x;
                    cm.Items.Add(questStop);
                } else if ("100%" == progress) {
                    MenuItem questClear = new MenuItem();
                    questClear.Click += new RoutedEventHandler(TakeQuest_Click);
                    QuestAction x = new QuestAction("完成", int.Parse(id), "clear");
                    questClear.Header = x;
                    cm.Items.Add(questClear);
                }
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        void TakeQuest_Click (object sender, RoutedEventArgs e) {
            MenuItem mi = e.OriginalSource as MenuItem;
            QuestAction qa = mi.Header as QuestAction;
            switch (qa.Action) {
                case "start":
                    RequestBuilder.Instance.QuestStart(qa.Id);
                    break;
                case "stop":
                    RequestBuilder.Instance.QuestStop(qa.Id);
                    break;
                case "clear":
                    RequestBuilder.Instance.QuestClear(qa.Id);
                    break;
                default:
                    break;
            }
            reflash();
        }

    }
}
