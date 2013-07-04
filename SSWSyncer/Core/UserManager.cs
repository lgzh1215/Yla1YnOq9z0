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
            users.Add(new UserInfo("chen.guan.hwa@gmail.com", "1qaz@WSX"));
            users.Add(new UserInfo("76450106@qq.com", "5462606"));
            users.Add(new UserInfo("sanakan0202@yahoo.com.tw", "74107410"));
            users.Add(new UserInfo("sephiroth8571@hotmail.com", "955593597"));
            users.Add(new UserInfo("qazwsx@yahoo.com", "qazwsxedcrfv"));
            users.Add(new UserInfo("weijack1524@hotmail.com", "jack1985"));
            users.Add(new UserInfo("76550001@qq.com", "123456"));
            users.Add(new UserInfo("s121@qq.com", "123123"));
            users.Add(new UserInfo("cwc12345@gmail.com", "cwc12345"));
            users.Add(new UserInfo("cwc54321@gmail.com", "cwc12345"));
            users.Add(new UserInfo("cwc12345cwc@gmail.com", "cwc12345"));
            users.Add(new UserInfo("hd0001@hd.com", "123456"));
            users.Add(new UserInfo("hd0002@hd.com", "123456"));
            users.Add(new UserInfo("hd0003@hd.com", "123456"));
            users.Add(new UserInfo("kin8591@hotmail.com", "livelihooh"));

            string password;
            // gfgf series
            password = "ssw342425";
            for (var i = 1; i <= 60; i++) {
                string account = "gfgf" + i + "zc@126.com";
                scriptlet.Add(new UserInfo(account, password));
            }
            // duo series
            password = "123123";
            for (var i = 100; i < 160; i++) {
                string account = "s" + i + "@qq.com";
                scriptlet.Add(new UserInfo(account, password));
            }
        }

    }
}
