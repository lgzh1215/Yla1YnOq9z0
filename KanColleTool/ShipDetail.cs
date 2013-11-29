﻿using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace KanColleTool {

    class ShipDetail {

        public JToken Spec { get; set; }

        public JToken Ship { get; set; }

        public JToken SType { get; set; }

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

        public double HPRatio {
            get {
                double r = Double.Parse(Ship["api_nowhp"].ToString()) / Double.Parse(Ship["api_maxhp"].ToString());
                //Debug.Print(Spec["api_name"].ToString() + " f% " + r.ToString());
                return r;
            }
        }

        public string FleetInfo {
            get {
                if (Ship["fleet_info"] == null) {
                    return "n/a";
                } else {
                    return Ship["fleet_info"].ToString();
                }
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
}
