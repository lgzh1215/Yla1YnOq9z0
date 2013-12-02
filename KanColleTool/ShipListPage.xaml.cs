using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace KanColleTool {
    /// <summary>
    /// ShipListPage.xaml 的互動邏輯
    /// </summary>
    public partial class ShipListPage : Page {

        public ShipListPage () {
            InitializeComponent();
        }

        private void Page_Loaded (object sender, RoutedEventArgs e) {
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
        }
        
    }

}
