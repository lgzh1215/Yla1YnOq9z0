using System;
using System.Collections.Generic;

namespace KanColleTool {

    class MissionDetail {

        public static readonly List<MissionDetail> All = new List<MissionDetail>();

        public static readonly Dictionary<int, int> IdMap = new Dictionary<int, int>();

        static MissionDetail () {
            All.Add(new MissionDetail(0, "0:出擊"));
            All.Add(new MissionDetail(1, "1:練習航海", "15m", 1, "駆×2"));
            All.Add(new MissionDetail(2, "2:長距離練習航海"));
            All.Add(new MissionDetail(3, "3:警備任務"));
            All.Add(new MissionDetail(4, "4:対潜警戒任務"));
            All.Add(new MissionDetail(5, "5:海上護衛任務"));
            All.Add(new MissionDetail(6, "6:防空射撃演習"));
            All.Add(new MissionDetail(7, "7:観艦式予行"));
            All.Add(new MissionDetail(8, "8:観艦式"));
            All.Add(new MissionDetail(9, "9:タンカー護衛任務"));
            All.Add(new MissionDetail(10, "10:強行偵察任務"));
            All.Add(new MissionDetail(11, "11:ボーキサイト輸送任務"));
            All.Add(new MissionDetail(12, "12:資源輸送任務"));
            All.Add(new MissionDetail(13, "13:鼠輸送作戦"));
            All.Add(new MissionDetail(14, "14:包囲陸戦隊撤収作戦"));
            All.Add(new MissionDetail(15, "15:囮機動部隊支援作戦"));
            All.Add(new MissionDetail(16, "16:艦隊決戦援護作戦"));
            All.Add(new MissionDetail(17, "17:敵地偵察作戦"));
            All.Add(new MissionDetail(18, "18:航空機輸送作戦"));
            All.Add(new MissionDetail(19, "19:北号作戦"));
            All.Add(new MissionDetail(20, "20:潜水艦哨戒任務"));
            All.Add(new MissionDetail(25, "25:通商破壊作戦"));
            All.Add(new MissionDetail(26, "26:敵母港空襲作戦"));
            All.Add(new MissionDetail(27, "27:潜水艦通商破壊作戦"));
            All.Add(new MissionDetail(28, "28:西方海域封鎖作戦"));
            All.Add(new MissionDetail(29, "29:潜水艦派遣演習"));
            All.Add(new MissionDetail(30, "30:潜水艦派遣作戦"));
            All.Add(new MissionDetail(33, "33:前衛支援任務"));
            All.Add(new MissionDetail(34, "34:艦隊決戦支援任務"));
            All.Add(new MissionDetail(35, "35:MO作戦"));
            All.Add(new MissionDetail(36, "36:水上機基地建設"));
            for (int i = 0; i < All.Count; i++) {
                IdMap.Add(All[i].Id, i);
            }
        }

        public string Name { get; private set; }

        public int Id { get; private set; }

        public string Time { get; private set; }

        public int FlagLv { get; private set; }

        public string Require { get; private set; }

        public string FlagLvInfo {
            get {
                if (FlagLv == 0) {
                    return "";
                } else {
                    return String.Format("必要LV: {0}", FlagLv);
                }
            }
        }

        private MissionDetail (int id, string name) {
            Id = id;
            Name = name;
        }

        private MissionDetail (int id, string name, string time, int flagLv, string require) {
            Id = id;
            Name = name;
            Time = time;
            FlagLv = flagLv;
            Require = require;
        }

        public override string ToString () {
            return Name;
        }

    }
}
