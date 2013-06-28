using System;
using System.Windows;
using WindowsInput.Native;

namespace SSWSyncer.Commands {

    [Serializable]
    class ClosePlanetMenuCommand : AbstractCommand {

        public override void Invoke (bool isSimulate) {
            log.Debug("Close planet menu");
            StateContainer.EnterPlanetary();
            if (isSimulate) {
                sim.Mouse.MoveMouseTo(487 * xf, 530 * yf).Sleep(100).LeftButtonClick().Sleep(1000);
            }
        }

        public override string ToString () {
            return "關閉行星功能表";
        }

    }

}
