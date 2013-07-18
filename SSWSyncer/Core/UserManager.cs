using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Common.Logging;

namespace SSWSyncer.Core {

    public class UserManager {

        private static ILog log = LogManager.GetLogger(typeof(UserManager));

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

        public int indexOf (string name) {
            for (int i = 0; i < scriptlet.Count; i++) {
                UserInfo info = scriptlet[i];
                if (info.Name == name) {
                    return i;
                }
            }
            return 0;
        }

        public void reLoadUsers () {
            users.Clear();
            scriptlet.Clear();
            loadUsers();
        }

        private UserManager () {
            loadUsers();
        }

        private void loadUsers () {
            try {
                var reader = new StreamReader(File.OpenRead(@"users.csv"), System.Text.Encoding.Default);
                reader.ReadLine();
                while (!reader.EndOfStream) {
                    try {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        UserInfo info;
                        if (values[3] == "1") {
                            info = new UserInfo(values[0], values[1], values[2], true);
                            scriptlet.Add(info);
                        } else {
                            info = new UserInfo(values[0], values[1], values[2], false);
                            users.Add(info);
                        }
                    } catch (Exception e) {
                        log.Error(e.Message, e);
                    }
                }
            } catch (Exception e) {
                log.Error(e.Message, e);
            }
        }

    }
}
