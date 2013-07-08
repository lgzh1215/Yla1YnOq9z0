using System;

namespace SSWSyncer.Commands {

    [Serializable]
    class LeavePlanetCommand : AbstractCommand {

        public override void Invoke (bool isSimulate) {
            log.Debug(this.ToString());
            StateContainer.EnterPlanetary();
            if (isSimulate) {
                sim.Mouse.MoveMouseTo(795 * xf, 79 * yf).Sleep(100).LeftButtonClick().Sleep(500);
            }
        }

        public override string ToString () {
            return "退出星球";
        }
    }

}
