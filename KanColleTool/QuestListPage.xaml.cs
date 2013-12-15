using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;

namespace KanColleTool {
    /// <summary>
    /// QuestList.xaml 的互動邏輯
    /// </summary>
    public partial class QuestListPage : Page {
        public QuestListPage () {
            InitializeComponent();
        }

        private void Page_Loaded (object sender, RoutedEventArgs e) {
            try {
                KCODt.Instance.QuestDataMap.Clear();
                RequestBuilder.Instance.QuestList(1);
                JToken result = KCODt.Instance.QuestData;
                Debug.Print("api_count:" + result["api_count"].ToString());
                QuestGrid.ItemsSource = null;
                QuestGrid.ItemsSource = KCODt.Instance.QuestDataMap.Values;
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }
    }
}
