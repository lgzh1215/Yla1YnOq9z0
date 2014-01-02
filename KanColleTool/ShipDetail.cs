using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.IO;

namespace KanColleTool {

    class ShipDetail {

        public JToken Spec { get; set; }

        public JToken Ship { get; set; }

        public JToken SType { get; set; }

        public double BullRatio {
            get {
                double r = Double.Parse(Ship["api_bull"].ToString()) / Double.Parse(Spec["api_bull_max"].ToString());
                return r;
            }
        }

        public double FuelRatio {
            get {
                double r = Double.Parse(Ship["api_fuel"].ToString()) / Double.Parse(Spec["api_fuel_max"].ToString());
                return r;
            }
        }

        public double HPRatio {
            get {
                double r = Double.Parse(Ship["api_nowhp"].ToString()) / Double.Parse(Ship["api_maxhp"].ToString());
                return r;
            }
        }

        public string FleetInfo {
            get {
                string key = Ship["api_id"].ToString();
                if (KCODt.Instance.NavalFleet.ContainsKey(key)) {
                    return KCODt.Instance.NavalFleet[key];
                } else {
                    return "n/a";
                }
            }
        }

        public string ShipIcoName {
            get {
                string filename = "";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images").Replace("\\", "/");
                string formatting = "file:///{0}/No.{1}-{2}{3}.png";
                string cond = "1";
                if (HPRatio <= 0.5) {
                    cond = "2";
                }
                filename = String.Format(formatting,
                    path, Spec["api_id"].ToString(), Spec["api_name"].ToString(), cond);
                return filename;
            }
        }

        public string HPRatioString {
            get {
                double r = HPRatio;
                if (r > 0 && r <= 0.25) {
                    return "大";
                } else if (r > 0.25 && r <= 0.5) {
                    return "中";
                } else if (r > 0.5 && r < 0.75) {
                    return "小";
                } else {
                    return "";
                }
            }
        }

        public ShipDetail (JToken spec, JToken ship, JToken stype) {
            Spec = spec;
            Ship = ship;
            SType = stype;
        }
    }

    class ShipSpecDetail {

        public JToken Spec {
            get;
            set;
        }

        public JToken SType { get; set; }

        public ShipSpecDetail (JToken spec, JToken stype) {
            Spec = spec;
            SType = stype;
        }

        public string ShipIcoName {
            get {
                string filename = "";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images").Replace("\\", "/");
                string formatting = "file:///{0}/No.{1}-{2}.png";
                filename = String.Format(formatting,
                    path, Spec["api_id"].ToString(), Spec["api_name"].ToString());
                return filename;
            }
        }
    }
}
