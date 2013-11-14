using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Drawing;
using System.Drawing.Imaging;

namespace SSWSyncer.Commands {

    [Serializable]
    class RecruitCommand : AbstractCommand {

        public override void Invoke (bool isSimulate, bool async) {
            log.Debug(this.ToString());
            bool loading = false;
            Bitmap bmpScreenshot;
            Graphics gfx;
            byte Luminosity;
            int tries;
            if (isSimulate) {
                sim.Mouse.MoveMouseTo(608 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(500);
                sim.Mouse.MoveMouseTo(252 * xf, 582 * yf).Sleep(100).LeftButtonClick();
                tries = 0;
                while (!loading && tries < 15) {
                    tries++;
                    bmpScreenshot = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
                    gfx = Graphics.FromImage(bmpScreenshot);
                    gfx.CopyFromScreen(595, 486, 0, 0,
                                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size,
                                CopyPixelOperation.SourceCopy);
                    Color color = bmpScreenshot.GetPixel(0, 0);
                    Luminosity = (byte) (color.GetBrightness() * 255);
                    log.Debug(Luminosity);
                    if (Luminosity > 230) {
                        loading = true;
                    }
                    sim.Mouse.Sleep(1000);
                }
                sim.Mouse.MoveMouseTo(600 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(500);
            }
        }

        public override string ToString () {
            return "招募指揮官";
        }

    }

}
