using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Drawing;
using System.Drawing.Imaging;

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

        public override void Invoke (bool isSimulate, bool async) {
            log.Debug(this.ToString());
            int x = 155;
            int y = 207 + (Index * 18) - 18;
            bool loading = false;
            bool fin = false;
            Bitmap bmpScreenshot;
            Graphics gfx;
            byte Luminosity;
            int tries;
            if (isSimulate) {
                if (Index <= 12) {
                    sim.Mouse.MoveMouseTo(x * xf, y * yf).Sleep(100).LeftButtonClick();
                    tries = 0;
                    while (!loading && tries < 60) {
                        tries++;
                        bmpScreenshot = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
                        gfx = Graphics.FromImage(bmpScreenshot);
                        gfx.CopyFromScreen(550, 650, 0, 0,
                                    System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size,
                                    CopyPixelOperation.SourceCopy);
                        Color color = bmpScreenshot.GetPixel(0, 0);
                        Luminosity = (byte) (color.GetBrightness() * 255);
                        log.Debug(Luminosity);
                        if (Luminosity > 20) {
                            loading = true;
                        }
                        sim.Mouse.Sleep(1000);
                    }
                    sim.Mouse.MoveMouseTo(570 * xf, 650 * yf).Sleep(100).LeftButtonClick();
                    tries = 0;
                    while (!fin && tries < 30) {
                        tries++;
                        bmpScreenshot = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
                        gfx = Graphics.FromImage(bmpScreenshot);
                        gfx.CopyFromScreen(612, 488, 0, 0,
                                    System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size,
                                    CopyPixelOperation.SourceCopy);
                        Color color = bmpScreenshot.GetPixel(0, 0);
                        Luminosity = (byte) (color.GetBrightness() * 255);
                        log.Debug(Luminosity);
                        if (Luminosity > 200) {
                            fin = true;
                        }
                        sim.Mouse.Sleep(1000);
                    }
                    sim.Mouse.MoveMouseTo(610 * xf, 492 * yf).Sleep(100).LeftButtonClick().Sleep(750);
                    sim.Mouse.MoveMouseTo(923 * xf, 767 * yf).Sleep(100).LeftButtonClick().Sleep(750);
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
