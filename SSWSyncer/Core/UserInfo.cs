using System;

namespace SSWSyncer.Core {

    [Serializable]
    public class UserInfo {

        public string Account { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public bool Rotate { get; set; }

        public UserInfo (string account, string password) {
            Account = account;
            Password = password;
        }

        public UserInfo (string account, string password, string name) {
            Account = account;
            Password = password;
            Name = name;
        }

        public UserInfo (string name, string account, string password, bool rotate) {
            Account = account;
            Password = password;
            Name = name;
            Rotate = rotate;
        }

        public override string ToString () {
            return Name + "\t" + Account + "\t" + Password + ")";
        }

    }
}
