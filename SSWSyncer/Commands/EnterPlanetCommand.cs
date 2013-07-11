using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SSWSyncer.Commands {

    [Serializable]
    class EnterPlanetCommand : AbstractCommand {

        public Point Point { get; set; }

        public bool FirstTime { get; private set; }

        public EnterPlanetCommand (Point point, bool firstTime) {
            Point = point;
            FirstTime = firstTime;
        }

        public EnterPlanetCommand (Dictionary<string, object> context) {
            ParserForm(context);
        }

        public override void Update (Dictionary<string, object> context) {
            ParserForm(context);
        }

        private void ParserForm (Dictionary<string, object> context) {
            TextBox txtPlanetMenuX = context["PointX"] as TextBox;
            TextBox txtPlanetMenuY = context["PointY"] as TextBox;
            CheckBox cb = context["FirstTime"] as CheckBox;
            Point point = new Point(Convert.ToInt32(txtPlanetMenuX.Text), Convert.ToInt32(txtPlanetMenuY.Text));
            Point = point;
            FirstTime = (cb.IsChecked == true);
        }

        public override void Invoke (bool isSimulate, bool async) {
            OpenPlanetMenuCommand openPlanetMenuCommand = new OpenPlanetMenuCommand(Point);
            openPlanetMenuCommand.StateContainer = StateContainer;
            openPlanetMenuCommand.Invoke(isSimulate, async);
            log.Debug(this.ToString());
            StateContainer.EnterPlanet();
            if (isSimulate) {
                sim.Mouse.MoveMouseBy(0, -25).Sleep(100).LeftButtonClick().Sleep(1000);
                if (FirstTime) {
                    sim.Mouse.LeftButtonClick().Sleep(100).LeftButtonClick().Sleep(100).LeftButtonClick().Sleep(100);
                }

                sim.Mouse.MoveMouseTo(500 * xf, 560 * yf);
                sim.Mouse.LeftButtonDown().Sleep(200);
                sim.Mouse.MoveMouseBy(0, -5).Sleep(200);
                sim.Mouse.LeftButtonUp().Sleep(200);

                sim.Mouse.MoveMouseTo(500 * xf, 560 * yf).LeftButtonClick().Sleep(800);
                sim.Mouse.LeftButtonDown().Sleep(800);
                sim.Mouse.MoveMouseTo(500 * xf, 330 * yf).Sleep(500);
                sim.Mouse.LeftButtonUp().Sleep(500);
            }
        }

        public override string ToString () {
            string result = "進入行星: (" + Point + ")";
            if (FirstTime) {
                result = "首次" + result;
            }
            return result;
        }

    }

}
