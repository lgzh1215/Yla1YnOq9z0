using SSWSyncer.Commands;

namespace SSWSyncer.States {

    class CommanderState : AbstractState {

        public CommanderState (StateContainer container)
            : base(container) {
        }

        public override void EnterGalaxy (Command command) {
            EnterGalaxy();
            container.Enqueue(command);
        }

        public override void EnterGalaxy () {
            container.ChangeState("GalaxyState");
        }

        public override void Logout (Command command) {
            Logout();
            container.Enqueue(command);
        }

        public override void Logout () {
            container.ChangeState("NotLoggedinState");
        }

        public override void EnterMissionPanel (Command command) {
            EnterMissionPanel();
            container.Enqueue(command);
        }

        public override void EnterMissionPanel () {
            container.ChangeState("MissionState");
        }

        public override string ToString () {
            return "指揮官列表";
        }

    }

}
