using System;

namespace SSWSyncer.Core {

    [Serializable]
    public class UserInfo {

        public string Account { get; set; }

        public string Password { get; set; }

        public UserInfo (string account, string password) {
            Account = account;
            Password = password;
        }

        public override string ToString () {
            return "使用者(" + Account + "/" + Password + ")";
        }

    }
}
