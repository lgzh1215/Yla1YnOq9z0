using System;

namespace SSWSyncer.Commands {

    [Serializable]
    class CloseFacilityBuildPanelCommand : AbstractCommand {

        public override void Invoke (bool isSimulate, bool async) {
            log.Debug(this.ToString());
            StateContainer.EnterPlanet();
            if (isSimulate) {
                sim.Mouse.MoveMouseTo(652 * xf, 658 * yf).Sleep(100).LeftButtonClick().Sleep(500);
            }
        }

        public override string ToString () {
            return "關閉設施建造列表";
        }
    }

}
