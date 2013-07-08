using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SSWSyncer.Commands {

    [Serializable]
    class EnterPlanetaryCommand : AbstractCommand {

        public Point Point { get; set; }

        public EnterPlanetaryCommand (Point point) {
            Point = point;
        }

        public EnterPlanetaryCommand (Dictionary<string, object> context) {
            ParserForm(context);
        }

        public override void Update (Dictionary<string, object> context) {
            ParserForm(context);
        }

        private void ParserForm (Dictionary<string, object> context) {
            TextBox txtPlanetaryX = context["PointX"] as TextBox;
            TextBox txtPlanetaryY = context["PointY"] as TextBox;
            Point point = new Point(Convert.ToInt32(txtPlanetaryX.Text), Convert.ToInt32(txtPlanetaryY.Text));
            Point = point;
        }

        public override void Invoke (bool isSimulate) {
            log.Debug(this.ToString());
            StateContainer.EnterPlanetary();
            if (isSimulate) {
                if (Point.X < StateContainer.ExtentMin.X || Point.Y < StateContainer.ExtentMin.Y ||
                    Point.X > StateContainer.ExtentMax.X || Point.Y < StateContainer.ExtentMax.Y) {
                    GalaxyMovingCommand moving = new GalaxyMovingCommand(Point);
                    moving.StateContainer = StateContainer;
                    moving.Invoke(isSimulate);
                }
                Point target = new Point();
                target.X = Point.X - StateContainer.ExtentMin.X + 15;
                target.Y = Point.Y - StateContainer.ExtentMin.Y + 67;
                sim.Mouse.MoveMouseTo(target.X * xf, target.Y * yf).Sleep(500);
                sim.Mouse.LeftButtonClick().Sleep(5000);
            }
        }

        public override string ToString () {
            return "進入行星系: (" + Point + ")";
        }

    }

}
