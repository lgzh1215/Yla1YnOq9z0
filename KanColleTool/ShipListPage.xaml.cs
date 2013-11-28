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
                if (KCODt.Ship2 != null && KCODt.Ship != null) {
                    var qm = from sh in KCODt.Ship["api_data"]
                             from s2 in KCODt.Ship2["api_data"]
                             where sh["api_id"].ToString() == s2["api_ship_id"].ToString()
                             select JToken.FromObject(new ship(sh, s2));
                    //var qm = from ss in KCODt.Ship["api_data"] where ss["api_stype"].ToString() == "4" select ss;
                    ShipGrid.ItemsSource = qm;
                    //ShipGrid.ItemsSource = KCODt.Ship["api_data"];
                }
            } catch (Exception ex) {
                Debug.Print(ex.Message);
            }

        }
    }

    class ship {
        public JToken Spec { get; set; }
        public JToken Ship { get; set; }
        public ship (JToken spec, JToken ship) {
            Spec = spec;
            Ship = ship;
        }
        public double BullRatio {
            get {
                double r = Double.Parse(Ship["api_bull"].ToString()) / Double.Parse(Spec["api_bull_max"].ToString());
                //Debug.Print(Spec["api_name"].ToString() + " b% " + r.ToString());
                return r;
            }
        }
        public double FuelRatio {
            get {
                double r = Double.Parse(Ship["api_fuel"].ToString()) / Double.Parse(Spec["api_fuel_max"].ToString());
                //Debug.Print(Spec["api_name"].ToString() + " f% " + r.ToString());
                return r;
            }
        }
    }
}
