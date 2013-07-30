using System;
using System.Collections.Generic;
using System.Windows.Controls;
using SSWSyncer.Core;
using WindowsInput.Native;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Media.Imaging;

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
            if (isSimulate) {
                int tries = 0;
                bool logined = false;
                sim.Mouse.MoveMouseTo(646 * xf, 580 * yf).Sleep(250).LeftButtonClick().Sleep(250);
                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A).Sleep(500);
                sim.Keyboard.TextEntry(UserInfo.Account).Sleep(500);
                sim.Mouse.MoveMouseTo(480 * xf, 610 * yf).Sleep(250).LeftButtonClick().Sleep(500);
                sim.Keyboard.TextEntry(UserInfo.Password).Sleep(500);
                sim.Mouse.MoveMouseTo(608 * xf, 644 * yf).Sleep(250).LeftButtonClick();
                while (!logined) {
                    tries++;
                    if (tries > 30) {
                        break;
                    }
                    sim.Mouse.Sleep(7500);
                    Bitmap bmpScreenshot = new Bitmap(145, 23, PixelFormat.Format32bppArgb);
                    Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                    gfxScreenshot.CopyFromScreen(32, 32, 0, 0,
                                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size,
                                CopyPixelOperation.SourceCopy);
                    logined = Utils.Compare(Utils.BitmapImage2Bitmap(loginProof), bmpScreenshot);
                    log.Debug("Loading...");
                }
            }
        }

        public override string ToString () {
            return "登入:" + UserInfo;
        }

    }

}
