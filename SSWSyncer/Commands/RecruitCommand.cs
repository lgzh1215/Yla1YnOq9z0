using System;

namespace SSWSyncer.Commands {

    [Serializable]
    class RecruitCommand : AbstractCommand {

        public override void Invoke (bool isSimulate) {
            log.Debug(this.ToString());
            if (isSimulate) {
                sim.Mouse.MoveMouseTo(608 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(500);
                sim.Mouse.MoveMouseTo(252 * xf, 582 * yf).Sleep(100).LeftButtonClick().Sleep(7500);
                sim.Mouse.MoveMouseTo(608 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(500);
            }
        }

        public override string ToString () {
            return "招募指揮官";
        }

    }

}
