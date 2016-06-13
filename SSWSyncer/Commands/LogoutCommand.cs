using System;

namespace SSWSyncer.Commands {

    [Serializable]
    class LogoutCommand : AbstractCommand {

        public override void Invoke (bool isSimulate, bool async) {
            log.Debug(this.ToString());
            StateContainer.Logout();
            if (isSimulate) {
                sim.Mouse.MoveMouseTo(1008 * xf, 16 * yf).Sleep(500).LeftButtonClick().Sleep(500);
            }
        }

        public override string ToString () {
            return "登出並關閉";
        }

    }
}
