using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace KanColleTool {

    class ItemDetail {

        public JToken Spec { get; set; }

        public JToken Item { get; set; }

        public string LengString {
            get {
                int leng = int.Parse(Spec["api_leng"].ToString());
                switch (leng) {
                    case 0:
                        return "-";
                    case 1:
                        return "短";
                    case 2:
                        return "中";
                    case 3:
                        return "長";
                    case 4:
                        return "超長";
                    default:
                        return "-";
                }
            }
        }

        public ItemDetail (JToken spec, JToken item) {
            Spec = spec;
            Item = item;
        }
    }
}
