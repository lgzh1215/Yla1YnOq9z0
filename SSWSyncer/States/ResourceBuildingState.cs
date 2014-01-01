using SSWSyncer.Commands;

namespace SSWSyncer.States {

    class ResourceBuildingState : AbstractState {

        public ResourceBuildingState (StateContainer container)
            : base(container) {
        }

        public override void EnterPlanet (Command command) {
            EnterPlanet();
            container.Enqueue(command);
        }

        public override void EnterPlanet () {
            container.ChangeState("PlanetState");
        }

        public override string ToString () {
            return "資源建造列表";
        }
    }
}
