using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using Common.Logging;

namespace SSWSyncer.Core {

    class Utils {

        static protected ILog log = LogManager.GetLogger(typeof(Utils));

        public static Bitmap BitmapImage2Bitmap (BitmapImage bitmapImage) {
            using (MemoryStream outStream = new MemoryStream()) {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                // return bitmap; <-- leads to problems, stream is closed/closing ...
                return new Bitmap(bitmap);
            }
        }

        public static bool Compare (Bitmap bmp1, Bitmap bmp2) {
            //CompareResult cr = CompareResult.ciCompareOk;
            //Test to see if we have the same size of image
            if (bmp1.Size != bmp2.Size) {
                log.Info("Size Mismatch");
                return false;
                //cr = CompareResult.ciSizeMismatch;
            } else {
                //Sizes are the same so start comparing pixels
                for (int x = 0; x < bmp1.Width; x++) {
                    for (int y = 0; y < bmp1.Height; y++) {
                        Color col1 = bmp1.GetPixel(x, y);
                        Color col2 = bmp2.GetPixel(x, y);
                        int absA = Math.Abs(col1.A - col2.A);
                        int absR = Math.Abs(col1.R - col2.R);
                        int absG = Math.Abs(col1.G - col2.G);
                        int absB = Math.Abs(col1.B - col2.B);
                        int deff = absA + absR + absG + absB;
                        if (deff > 10) {
                            log.Info("Pixel Mismatch @(" + x + ", " + y + ")");
                            log.Info("col1: " + col1.ToString());
                            log.Info("col2: " + col2.ToString());
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        //Bitmap bmpScreenshot = new Bitmap(145, 23, PixelFormat.Format32bppArgb);
        //Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);
        //gfxScreenshot.CopyFromScreen(32, 32, 0, 0,
        //            System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size,
        //            CopyPixelOperation.SourceCopy);
        //bmpScreenshot.Save("aaa.bmp", ImageFormat.Bmp);

        //sim.Mouse.MoveMouseTo(608 * xf, 644 * yf);
    }
}
