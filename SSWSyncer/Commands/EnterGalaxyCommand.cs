using System;

namespace SSWSyncer.Commands {

    [Serializable]
    class EnterGalaxyCommand : AbstractCommand {

        public override void Invoke (bool isSimulate) {
            log.Debug("Enter galaxy");
            StateContainer.EnterGalaxy();
            if (isSimulate) {
                sim.Mouse.MoveMouseTo(144 * xf, 44 * yf).Sleep(500).LeftButtonClick().Sleep(500);
            }
        }

        public override string ToString () {
            return "進入銀河系";
        }

    }

}
