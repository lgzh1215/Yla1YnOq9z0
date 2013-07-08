using System;

namespace SSWSyncer.Commands {

    [Serializable]
    class OpenMissionPanelCommand : AbstractCommand {

        public OpenMissionPanelCommand () {
        }

        public override void Invoke (bool isSimulate) {
            log.Debug(this.ToString());
            StateContainer.EnterMissionPanel();
            if (isSimulate) {
                sim.Mouse.MoveMouseTo(923 * xf, 767 * yf).Sleep(100).LeftButtonClick().Sleep(500);
            }
        }

        public override string ToString () {
            return "打開任務列表";
        }
    }

}
