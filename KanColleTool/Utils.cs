using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanColleTool {

    static public class Utils {

        static public string valueOfUTC (string value) {
            string result = "";
            if (value != "" && value != "0") {
                DateTime date = parseUTC(value);
                result = date.ToString("HH:mm:ss");
            }
            return result;
        }

        static public DateTime parseUTC (string value) {
            long utc = long.Parse(value);
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(utc).ToLocalTime();
            return date;
        }

        static public TimeSpan countSpan (string value) {
            DateTime eta = parseUTC(value);
            TimeSpan span = eta - DateTime.Now;
            return span;
        }

    }
}
