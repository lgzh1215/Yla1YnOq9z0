using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace KanColleTool {

    class QuestDetail {

        public JToken Data { get; set; }

        public string Progress {
            get {
                string result = "";
                string progress = Data["api_progress_flag"].ToString();
                switch (progress) {
                    case "1":
                        result = "50%";
                        break;
                    case "2":
                        result = "80%";
                        break;
                    case "0":
                        if (Data["api_state"].ToString() == "3") {
                            result = "100%";
                        }
                        break;
                    default:
                        break;
                }
                return result;
            }
        }

        public string Status {
            get {
                string result = "";
                string state = Data["api_state"].ToString();
                switch (state) {
                    case "2":
                        result = "執行中";
                        break;
                    case "3":
                        result = "達成";
                        break;
                    default:
                        break;
                }
                return result;
            }
        }

        public QuestDetail (JToken data) {
            Data = data;
        }
    }

    class QuestAction {

        public string Title { get; private set; }

        public int Id { get; private set; }

        public string Action { get; private set; }

        public QuestAction (string title, int id, string action) {
            Title = title;
            Id = id;
            Action = action;
        }

        public override string ToString () {
            return Title;
        }
    }
}
