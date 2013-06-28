using SSWSyncer.Commands;

namespace SSWSyncer.States {

    class GalaxyState : AbstractState {

        public GalaxyState (StateContainer container)
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

        public override void EnterMissionPanel (Command command) {
            EnterMissionPanel();
            container.Enqueue(command);
        }

        public override void EnterMissionPanel () {
            container.ChangeState("MissionState");
        }

        public override void EnterPlanetary (Command command) {
            EnterPlanetary();
            container.Enqueue(command);
        }

        public override void EnterPlanetary () {
            container.ChangeState("PlanetaryState");
        }

        public override string ToString () {
            return "在銀河系";
        }
    }

}
