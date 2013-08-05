using System;
using System.Windows.Forms;
using System.Drawing;

namespace SSWSyncer.Commands {

    [Serializable]
    class DismissCommand : AbstractCommand {

        public override void Invoke (bool isSimulate, bool async) {
            Point lastMousePosition = Cursor.Position;
            log.Debug(this.ToString());
            if (isSimulate) {
                sim.Mouse.MoveMouseTo(650 * xf, 580 * yf).Sleep(300).LeftButtonClick().Sleep(300);
                sim.Mouse.MoveMouseTo(500 * xf, 460 * yf).Sleep(300).LeftButtonClick().Sleep(300);
                sim.Mouse.MoveMouseTo(lastMousePosition.X * xf, (lastMousePosition.Y + 1)* yf);
            }
        }

        public override string ToString () {
            return "解雇指揮";
        }

    }
}
