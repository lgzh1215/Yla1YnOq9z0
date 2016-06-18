using System;

namespace SSWSyncer.Commands {

    [Serializable]
    class LogoutCommand : AbstractCommand {

        public override void Invoke (bool isSimulate, bool async) {
            log.Debug(this.ToString());
            StateContainer.Logout();
            if (isSimulate) {
                bool enabled;
                Boolean.TryParse(StateContainer.ini.Read("changeMac", "Settings"), out enabled);
                if (enabled) {
                    sim.Mouse.MoveMouseTo(1000 * xf, 765 * yf).Sleep(500).LeftButtonClick().Sleep(1500);
                    sim.Mouse.MoveMouseTo(184 * xf, 672 * yf).Sleep(500).LeftButtonClick().Sleep(500);
                } else {
                    string cx = StateContainer.ini.Read("SSWClientX", "Settings");
                    string cy = StateContainer.ini.Read("SSWClientY", "Settings");
                    sim.Mouse.MoveMouseTo(1008 * xf, 16 * yf).Sleep(500).LeftButtonClick().Sleep(500);
                    sim.Mouse.MoveMouseTo(int.Parse(cx) * xf, int.Parse(cy) * yf).Sleep(500).LeftButtonDoubleClick().Sleep(500);
                }
            }
        }

        public override string ToString () {
            return "登出";
        }

    }
}
