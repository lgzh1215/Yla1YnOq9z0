using SSWSyncer.Commands;
using System.Windows;

namespace SSWSyncer.States {

    class NotLoggedinState : AbstractState {

        public NotLoggedinState (StateContainer container)
            : base(container) {
        }

        public override void Login (Command command) {
            Login();
            container.Enqueue(command);
        }

        public override void Login () {
            container.ExtentMin = new Point(0, 0);
            container.ExtentMax = new Point(790, 660);
            container.ChangeState("GalaxyState");
        }

        public override string ToString () {
            return "尚未登入";
        }
    }
}
