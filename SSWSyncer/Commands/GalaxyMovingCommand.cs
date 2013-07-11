using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SSWSyncer.Commands {

    [Serializable]
    class GalaxyMovingCommand : AbstractCommand {

        public Point Point { get; set; }

        public GalaxyMovingCommand (Point point) {
            Point = point;
        }

        public GalaxyMovingCommand (Dictionary<string, object> context) {
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

        public override void Invoke (bool isSimulate, bool async) {
            log.Debug(this.ToString());
            // M874 = 114 * xf, 167 * yf
            // zero point = 14, 67
            // box = 790 x 660
            if (isSimulate) {
                // move x axis
                bool isPositiveX = false;
                double measureX = Point.X - StateContainer.ExtentMax.X;
                double stepsX = 0;
                if (Point.X > StateContainer.ExtentMax.X) {
                    isPositiveX = true;
                    stepsX = Math.Ceiling(Math.Abs(measureX / 700));
                }
                if (Point.X < StateContainer.ExtentMin.X) {
                    stepsX = Math.Floor(Math.Abs(measureX / 700));
                }
                for (int i = 0; i < stepsX; i++) {
                    Point a;
                    Point b;
                    var extentMax = StateContainer.ExtentMax;
                    var extentMin = StateContainer.ExtentMin;
                    if (isPositiveX) {
                        a = new Point(750, 100);
                        b = new Point(50, 100);
                        extentMax.X += 700;
                        extentMin.X += 700;
                    } else {
                        a = new Point(50, 100);
                        b = new Point(750, 100);
                        extentMax.X -= 700;
                        extentMin.X -= 700;
                    }
                    StateContainer.ExtentMax = extentMax;
                    StateContainer.ExtentMin = extentMin;
                    sim.Mouse.MoveMouseTo(a.X * xf, a.Y * yf).Sleep(200);
                    sim.Mouse.LeftButtonDown().Sleep(800);
                    sim.Mouse.MoveMouseTo(b.X * xf, b.Y * yf).Sleep(200);
                    sim.Mouse.LeftButtonUp();
                    log.Debug("step:" + i);
                }
                // move y axis
                bool isPositiveY = false;
                double measureY = Point.Y - StateContainer.ExtentMax.Y;
                double stepsY = 0;
                if (Point.Y > StateContainer.ExtentMax.Y) {
                    isPositiveY = true;
                    stepsY = Math.Ceiling(Math.Abs(measureY / 550));
                }
                if (Point.Y < StateContainer.ExtentMin.Y) {
                    stepsY = Math.Floor(Math.Abs(measureY / 550));
                }
                for (int i = 0; i < stepsY; i++) {
                    Point a;
                    Point b;
                    var extentMax = StateContainer.ExtentMax;
                    var extentMin = StateContainer.ExtentMin;
                    if (isPositiveY) {
                        a = new Point(60, 680);
                        b = new Point(60, 130);
                        extentMax.Y += 550;
                        extentMin.Y += 550;
                    } else {
                        a = new Point(60, 130);
                        b = new Point(60, 680);
                        extentMax.Y -= 550;
                        extentMin.Y -= 550;
                    }
                    StateContainer.ExtentMax = extentMax;
                    StateContainer.ExtentMin = extentMin;
                    sim.Mouse.MoveMouseTo(a.X * xf, a.Y * yf).Sleep(200);
                    sim.Mouse.LeftButtonDown().Sleep(800);
                    sim.Mouse.MoveMouseTo(b.X * xf, b.Y * yf).Sleep(200);
                    sim.Mouse.LeftButtonUp();
                    log.Debug("step:" + i);
                }
            }
        }

        public override string ToString () {
            return "移動到: (" + Point + ")";
        }

    }

}
