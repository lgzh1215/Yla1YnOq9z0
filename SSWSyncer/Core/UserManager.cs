using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSWSyncer.Core {

    public class UserManager {

        private List<UserInfo> users = new List<UserInfo>();

        private List<UserInfo> scriptlet = new List<UserInfo>();

        private static UserManager instance = null;

        public List<UserInfo> Users {
            get {
                return users;
            }
        }

        public UserInfo SSKSeriesHead { get; private set; }

        public UserInfo FGSeriesHead { get; private set; }

        public UserInfo SESeriesHead { get; private set; }

        public List<UserInfo> Scriptlet {
            get {
                return scriptlet;
            }
        }

        public static UserManager getInstance () {
            if (instance == null) {
                instance = new UserManager();
            }
            return instance;
        }

        private UserManager () {
            users.Add(new UserInfo("chen.guan.hwa@gmail.com", "1qaz@WSX", "Seele"));
            users.Add(new UserInfo("qazwsx@yahoo.com", "qazwsxedcrfv", "米腸"));
            users.Add(new UserInfo("cw2cheng@yahoo.com", "latrobe", "威鳥"));
            users.Add(new UserInfo("sanakan0202@yahoo.com.tw", "74107410", "可樂"));
            users.Add(new UserInfo("sephiroth8571@hotmail.com", "955593597", "全家"));
            users.Add(new UserInfo("weijack1524@hotmail.com", "jack1985", "FIA"));
            users.Add(new UserInfo("76550001@qq.com", "123456", "SE十刃"));
            users.Add(new UserInfo("s100@qq.com", "123123", "s100"));
            users.Add(new UserInfo("gfgf1zc@126.com", "ssw342425", "FG1"));
            users.Add(new UserInfo("cwc12345@gmail.com", "cwc12345", "月映"));
            users.Add(new UserInfo("76450106@qq.com", "5462606", "小G"));

            string account;
            string password;
            string name;
            UserInfo info;

            // duo series
            password = "123123";
            for (var i = 100; i < 160; i++) {
                account = "s" + i + "@qq.com";
                name = "s" + i;
                info = new UserInfo(account, password, name);
                scriptlet.Add(info);
                if (i == 100) {
                    SSKSeriesHead = info;
                }
            }

            // gfgf series
            password = "ssw342425";
            for (var i = 1; i <= 60; i++) {
                account = "gfgf" + i + "zc@126.com";
                name = "FG" + i;
                if (i == 5 || i == 54 || i == 55) {
                    continue;
                }
                info = new UserInfo(account, password, name);
                scriptlet.Add(info);
                if (i == 1) {
                    FGSeriesHead = info;
                }
            }

            // SE series
            password = "123456";
            for (var i = 1; i <= 10; i++) {
                account = "765500" + string.Format("{0:00}", i) + "@qq.com";
                name = "se" + i;
                info = new UserInfo(account, password, name);
                scriptlet.Add(info);
                if (i == 1) {
                    SESeriesHead = info;
                }
            }
        }

    }
}
