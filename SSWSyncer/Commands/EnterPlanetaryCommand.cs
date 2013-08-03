using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using SSWSyncer.Core;


namespace SSWSyncer.Commands {

    [Serializable]
    class EnterPlanetaryCommand : AbstractCommand {

        private static BitmapImage planetaryProof = new BitmapImage(new Uri("pack://application:,,,/Images/planetaryProof.bmp"));

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

        public override void Invoke (bool isSimulate, bool async) {
            log.Debug(this.ToString());
            StateContainer.EnterPlanetary();
            int tries = 0;
            bool loading = false;
            System.Drawing.Bitmap bmpScreenshot;
            System.Drawing.Graphics gfx;
            if (isSimulate) {
                if (Point.X < StateContainer.ExtentMin.X || Point.Y < StateContainer.ExtentMin.Y ||
                    Point.X > StateContainer.ExtentMax.X || Point.Y < StateContainer.ExtentMax.Y) {
                    GalaxyMovingCommand moving = new GalaxyMovingCommand(Point);
                    moving.StateContainer = StateContainer;
                    moving.Invoke(isSimulate, async);
                }
                Point target = new Point();
                target.X = Point.X - StateContainer.ExtentMin.X + 15;
                target.Y = Point.Y - StateContainer.ExtentMin.Y + 67;
                sim.Mouse.MoveMouseTo(target.X * xf, target.Y * yf).Sleep(500);
                sim.Mouse.LeftButtonClick();
                while (!loading) {
                    tries++;
                    if (tries > 30) {
                        break;
                    }
                    sim.Mouse.Sleep(5000);
                    bmpScreenshot = new System.Drawing.Bitmap(145, 23, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    gfx = System.Drawing.Graphics.FromImage(bmpScreenshot);
                    gfx.CopyFromScreen(32, 32, 0, 0,
                                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size,
                                System.Drawing.CopyPixelOperation.SourceCopy);
                    loading = Utils.Compare(Utils.BitmapImage2Bitmap(planetaryProof), bmpScreenshot);
                    log.Debug("Loading...");
                    if (loading) {
                        break;
                    }
                }
            }
        }

        public override string ToString () {
            return "進入行星系: (" + Point + ")";
        }

    }

}
