using System;

namespace SSWSyncer.Commands {

    [Serializable]
    class OpenCommanderPanelCommand : AbstractCommand {

        public override void Invoke (bool isSimulate) {
            log.Debug(this.ToString());
            StateContainer.EnterCommanderPanel();
            if (isSimulate) {
                sim.Mouse.MoveMouseTo(850 * xf, 710 * yf).Sleep(100).LeftButtonClick().Sleep(2000);
            }
        }

        public override string ToString () {
            return "打開指揮官列表";
        }

    }

}
