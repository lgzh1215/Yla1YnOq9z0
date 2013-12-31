using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace KanColleTool {

    public class NavigateDetail {

        public string Type { get; private set; }

        public JToken Json { get; private set; }

        public DateTime Time { get; private set; }

        public NavigateDetail (string type, JToken json) {
            Type = type;
            Json = json;
            Time = DateTime.Now;
        }
    }
}
