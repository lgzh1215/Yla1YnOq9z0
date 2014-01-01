using SSWSyncer.Commands;

namespace SSWSyncer.States {

    class PlanetMenuState : AbstractState {

        public PlanetMenuState (StateContainer container)
            : base(container) {
        }

        public override void EnterPlanet (Command command) {
            EnterPlanet();
            container.Enqueue(command);
        }

        public override void EnterPlanet () {
            container.ChangeState("PlanetState");
        }

        public override void EnterPlanetary (Command command) {
            EnterPlanetary();
            container.Enqueue(command);
        }

        public override void EnterPlanetary () {
            container.ChangeState("PlanetaryState");
        }

        public override string ToString () {
            return "星球功能表被打開";
        }
    }
}
