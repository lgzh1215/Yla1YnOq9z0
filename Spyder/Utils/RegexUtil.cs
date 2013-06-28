using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Spyder.Utils {
    class RegexUtil {

        public static IEnumerable Matches (String pPattern, String pText, int pGroupId) {
            Regex r = new Regex(pPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            for (Match m = r.Match(pText); m.Success; m = m.NextMatch()) {
                yield return m.Groups[pGroupId].Value;
            }
        }

        public static string MatcheGroups (String pPattern, String pText, int pGroupId) {
            Regex r = new Regex(pPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Match m = r.Match(pText);
            string result = "";
            if (m.Success) {
                result = m.Groups[pGroupId].Value;
            }
            return result;
        }

        public static GroupCollection MatcheGetGroup (String pPattern, String pText) {
            Regex r = new Regex(pPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Match m = r.Match(pText);
            GroupCollection result = null;
            if (m.Success) {
                result = m.Groups;
            }
            return result;
        }
    }
}
