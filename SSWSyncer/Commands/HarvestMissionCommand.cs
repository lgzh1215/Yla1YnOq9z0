using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SSWSyncer.Commands {

    [Serializable]
    class HarvestMissionCommand : AbstractCommand {

        public int Index { get; private set; }

        public HarvestMissionCommand (int index) {
            Index = index;
        }

        public HarvestMissionCommand (Dictionary<string, object> context) {
            ParserForm(context);
        }

        public override void Update (Dictionary<string, object> context) {
            ParserForm(context);
        }

        private void ParserForm (Dictionary<string, object> context) {
            TextBox txtHarvest = context["Index"] as TextBox;
            Index = Convert.ToInt32(txtHarvest.Text);
        }

        public override void Invoke (bool isSimulate) {
            log.Debug(this.ToString());
            int x = 155;
            int y = 207 + (Index * 18) - 18;
            if (isSimulate) {
                if (Index <= 9) {
                    sim.Mouse.MoveMouseTo(x * xf, y * yf).Sleep(100).LeftButtonClick().Sleep(2000);
                    sim.Mouse.MoveMouseTo(570 * xf, 650 * yf).Sleep(100).LeftButtonClick().Sleep(2000);
                    sim.Mouse.MoveMouseTo(610 * xf, 492 * yf).Sleep(100).LeftButtonClick().Sleep(500);
                    sim.Mouse.MoveMouseTo(923 * xf, 767 * yf).Sleep(100).LeftButtonClick().Sleep(500);
                } else {
                    // TODO
                    throw new NotSupportedException();
                }
            }
        }

        public override string ToString () {
            return "完成第" + Index + "項任務";
        }

    }

}
