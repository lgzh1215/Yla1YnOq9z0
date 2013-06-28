using SSWSyncer.Commands;

namespace SSWSyncer.States {

    class PlanetState : AbstractState {

        public PlanetState (StateContainer container)
            : base(container) {
        }

        public override void EnterPlanetary (Command command) {
            EnterPlanetary();
            container.Enqueue(command);
        }

        public override void EnterPlanetary () {
            container.ChangeState("PlanetaryState");
        }

        public override void EnterResourceBuildingList (Command command) {
            EnterResourceBuildingList();
            container.Enqueue(command);
        }

        public override void EnterResourceBuildingList () {
            container.ChangeState("ResourceBuildingState");
        }

        public override string ToString () {
            return "已進入星球";
        }
    }
}
