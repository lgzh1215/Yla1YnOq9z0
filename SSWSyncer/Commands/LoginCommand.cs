using System;
using System.Collections.Generic;
using System.Windows.Controls;
using SSWSyncer.Core;
using WindowsInput.Native;

namespace SSWSyncer.Commands {

    [Serializable]
    class LoginCommand : AbstractCommand {

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

        public override void Invoke (bool isSimulate) {
            log.Debug(this.ToString());
            StateContainer.Login();
            if (isSimulate) {
                sim.Mouse.MoveMouseTo(646 * xf, 580 * yf).Sleep(250).LeftButtonClick().Sleep(250);
                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A).Sleep(500);
                sim.Keyboard.TextEntry(UserInfo.Account).Sleep(500);
                sim.Mouse.MoveMouseTo(480 * xf, 610 * yf).Sleep(250).LeftButtonClick().Sleep(500);
                sim.Keyboard.TextEntry(UserInfo.Password).Sleep(500);
                sim.Mouse.MoveMouseTo(608 * xf, 644 * yf).Sleep(250).LeftButtonClick();
            }
        }

        public override string ToString () {
            return "登入:" + UserInfo;
        }

    }

}
