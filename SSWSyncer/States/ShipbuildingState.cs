using SSWSyncer.Commands;

namespace SSWSyncer.States {

    class ShipbuildingState : AbstractState {

        public ShipbuildingState (StateContainer container)
            : base(container) {
        }

        public override void OpenPlanetMenu (Command command) {
            OpenPlanetMenu();
            container.Enqueue(command);
        }

        public override void OpenPlanetMenu () {
            container.ChangeState("PlanetMenuState");
        }

        public override string ToString () {
            return "造艦列表";
        }
    }
}
