using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SSWSyncer.Commands {

    [Serializable]
    class FleetCommand : AbstractCommand {

        public int Index { get; private set; }

        public FleetCommand (int index) {
            Index = index;
        }

        public FleetCommand (Dictionary<string, object> context) {
            ParserForm(context);
        }

        public override void Update (Dictionary<string, object> context) {
            ParserForm(context);
        }

        private void ParserForm (Dictionary<string, object> context) {
            TextBox txtFleet = context["Index"] as TextBox;
            Index = Convert.ToInt32(txtFleet.Text);
        }

        public override void Invoke (bool isSimulate) {
            log.Debug("Fleet mission: " + Index);
            int x = 200;
            int y = 260 + (Index * 34) - 34;
            if (isSimulate) {
                if (Index <= 5) {
                    sim.Mouse.MoveMouseTo(990 * xf, 654 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    sim.Mouse.MoveMouseTo(x * xf, y * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    sim.Mouse.MoveMouseTo(300 * xf, 520 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    sim.Mouse.MoveMouseTo(370 * xf, 470 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    sim.Mouse.MoveMouseTo(525 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    sim.Mouse.MoveMouseTo(380 * xf, 210 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    sim.Keyboard.TextEntry("a").Sleep(100);
                    sim.Mouse.MoveMouseTo(570* xf, 670 * yf).Sleep(100).LeftButtonClick().Sleep(600);
                    sim.Mouse.MoveMouseTo(566 * xf, 675 * yf).Sleep(100).LeftButtonClick().Sleep(600);
                    sim.Mouse.MoveMouseTo(661 * xf, 646 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                } else {
                    // TODO
                    throw new NotSupportedException();
                }
            }
        }

        public override string ToString () {
            return "創立艦隊" + Index + ", type A";
        }

    }

}
