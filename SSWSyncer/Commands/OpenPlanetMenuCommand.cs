using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SSWSyncer.Commands {

    [Serializable]
    class OpenPlanetMenuCommand : AbstractCommand {

        public Point Point { get; set; }

        public OpenPlanetMenuCommand (Point point) {
            Point = point;
        }

        public OpenPlanetMenuCommand (Dictionary<string, object> context) {
            ParserForm(context);
        }

        public override void Update (Dictionary<string, object> context) {
            ParserForm(context);
        }

        private void ParserForm (Dictionary<string, object> context) {
            TextBox txtPlanetMenuX = context["PointX"] as TextBox;
            TextBox txtPlanetMenuY = context["PointY"] as TextBox;
            Point point = new Point(Convert.ToInt32(txtPlanetMenuX.Text), Convert.ToInt32(txtPlanetMenuY.Text));
            Point = point;
        }

        public override void Invoke (bool isSimulate) {
            // zero = 14px, 56px
            // M874IV= (17, 15)
            log.Debug("Open planet menu: " + Point);
            StateContainer.OpenPlanetMenu();
            if (isSimulate) {
                Point target = new Point();
                Point shift = new Point(7, 18);
                target.X = 45 * Point.X + 14 + shift.X;
                target.Y = 45 * Point.Y + 56 + shift.Y;
                sim.Mouse.MoveMouseTo(target.X * xf, target.Y * yf).Sleep(500).LeftButtonClick().Sleep(2000);
            }
        }

        public override string ToString () {
            return "打開行星功能表: (" + Point + ")";
        }

    }

}
