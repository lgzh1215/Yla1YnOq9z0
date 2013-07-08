using System;

namespace SSWSyncer.Commands {

    [Serializable]
    class CloseCommanderPanelCommand : AbstractCommand {

        public override void Invoke (bool isSimulate) {
            log.Debug(this.ToString());
            StateContainer.EnterGalaxy();
            if (isSimulate) {
                sim.Mouse.MoveMouseTo(654 * xf, 612 * yf).Sleep(100).LeftButtonClick().Sleep(500);
            }
        }

        public override string ToString () {
            return "關閉指揮官列表";
        }
    }

}
