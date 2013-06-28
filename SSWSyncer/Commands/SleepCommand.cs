using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SSWSyncer.Commands {

    [Serializable]
    class SleepCommand : AbstractCommand {

        public int Second { get; private set; }

        public SleepCommand (int second) {
            Second = second;
        }

        public SleepCommand (Dictionary<string, object> context) {
            ParserForm(context);
        }

        public override void Update (Dictionary<string, object> context) {
            ParserForm(context);
        }

        private void ParserForm (Dictionary<string, object> context) {
            TextBox txtSecond = context["Second"] as TextBox;
            Second = Convert.ToInt32(txtSecond.Text);
        }

        public override void Invoke (bool isSimulate) {
            log.Debug("Sleep: " + Second);
            if (isSimulate) {
                sim.Mouse.Sleep(Second * 1000);
            }
        }

        public override string ToString () {
            return "等待" + Second + "秒";
        }
    }

}
