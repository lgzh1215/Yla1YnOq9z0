using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using SSWSyncer.Core;
using WindowsInput.Native;

namespace SSWSyncer.Commands {

    [Serializable]
    class LoginCommand : AbstractCommand {

        private static BitmapImage loginProof = new BitmapImage(new Uri("pack://application:,,,/Images/loginProof.bmp"));

        public UserInfo UserInfo { get; set; }

        public LoginCommand (UserInfo userInfo) {
            UserInfo = userInfo;
        }

        public LoginCommand (Dictionary<string, object> context) {
            ParserForm(context);
        }

        public override void Update (Dictionary<string, object> context) {
            ParserForm(context);
        }

        private void ParserForm (Dictionary<string, object> context) {
            ComboBox Combobox = context["UserInfo"] as ComboBox;
            UserInfo selected = Combobox.SelectedItem as UserInfo;
            UserInfo = selected;
        }

        public override void Invoke (bool isSimulate, bool async) {
            log.Debug(this.ToString());
            StateContainer.Login();
            int tries = 0;
            bool loading = false;
            bool signIn = false;
            Bitmap bmpScreenshot;
            Graphics gfx;
            byte Luminosity;
            if (isSimulate) {
                while (!loading) {
                    if (!signIn) {
                        doLogin();
                        signIn = true;
                    }
                    tries++;
                    if (tries > 30) {
                        break;
                    }
                    sim.Mouse.Sleep(5000);
                    bmpScreenshot = new Bitmap(145, 23, PixelFormat.Format32bppArgb);
                    gfx = Graphics.FromImage(bmpScreenshot);
                    gfx.CopyFromScreen(32, 32, 0, 0,
                                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size,
                                CopyPixelOperation.SourceCopy);
                    loading = Utils.Compare(Utils.BitmapImage2Bitmap(loginProof), bmpScreenshot);
                    log.Debug("Loading...");
                    if (loading) {
                        break;
                    }
                    bmpScreenshot = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
                    gfx = Graphics.FromImage(bmpScreenshot);
                    gfx.CopyFromScreen(590, 490, 0, 0,
                                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size,
                                CopyPixelOperation.SourceCopy);
                    Color color = bmpScreenshot.GetPixel(0, 0);
                    Luminosity = (byte) (color.GetBrightness() * 255);
                    log.Debug("signIn color.Lum:" + Luminosity);
                    if (Luminosity < 120) {
                        sim.Mouse.MoveMouseTo(590 * xf, 490 * yf).Sleep(250).LeftButtonClick();
                        signIn = false;
                        continue;
                    }
                }
            }
        }

        public override string ToString () {
            return "登入:" + UserInfo;
        }

        private void doLogin () {
            sim.Mouse.MoveMouseTo(590 * xf, 580 * yf).Sleep(250).LeftButtonClick().Sleep(250);
            sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A).Sleep(500);
            sim.Keyboard.TextEntry(UserInfo.Account).Sleep(500);
            sim.Mouse.MoveMouseTo(480 * xf, 610 * yf).Sleep(250).LeftButtonClick().Sleep(500);
            sim.Keyboard.TextEntry(UserInfo.Password).Sleep(500);
            sim.Mouse.MoveMouseTo(600 * xf, 644 * yf).Sleep(250).LeftButtonClick();
        }

    }

}
