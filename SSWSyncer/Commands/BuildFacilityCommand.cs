using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SSWSyncer.Commands {

    [Serializable]
    class BuildFacilityCommand : AbstractCommand {

        public int Index { get; private set; }

        public BuildFacilityCommand (int index) {
            Index = index;
        }

        public BuildFacilityCommand (Dictionary<string, object> context) {
            ParserForm(context);
        }

        public override void Update (Dictionary<string, object> context) {
            ParserForm(context);
        }

        private void ParserForm (Dictionary<string, object> context) {
            TextBox txtHarvest = context["Index"] as TextBox;
            Index = Convert.ToInt32(txtHarvest.Text);
        }

        public override void Invoke (bool isSimulate, bool async) {
            log.Debug(this.ToString());
            int x = 355;
            int y = 190 + (Index * 47) - 47;
            if (isSimulate) {
                if (Index <= 5) {
                    sim.Mouse.MoveMouseTo(x * xf, y * yf).Sleep(100).LeftButtonClick().Sleep(1000).LeftButtonClick();
                } else {
                    // TODO
                    throw new NotSupportedException();
                }
            }
        }

        public override string ToString () {
            return "建造第" + Index + "項建築";
        }
    }

}
