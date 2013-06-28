using SSWSyncer.Commands;

namespace SSWSyncer.States {

    class MissionState : AbstractState {

        public MissionState (StateContainer container)
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

        public override void EnterCommanderPanel (Command command) {
            EnterCommanderPanel();
            container.Enqueue(command);
        }

        public override void EnterCommanderPanel () {
            container.ChangeState("CommanderState");
        }

        public override void EnterPlanetary (Command command) {
            EnterPlanetary();
            container.Enqueue(command);
        }

        public override void EnterPlanetary () {
            container.ChangeState("PlanetaryState");
        }

        public override string ToString () {
            return "任務列表";
        }

    }

}
