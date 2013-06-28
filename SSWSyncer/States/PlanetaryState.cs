using SSWSyncer.Commands;

namespace SSWSyncer.States {

    class PlanetaryState : AbstractState {

        public PlanetaryState (StateContainer container)
            : base(container) {
        }

        public override void EnterGalaxy (Command command) {
            EnterGalaxy();
            container.Enqueue(command);
        }

        public override void EnterGalaxy () {
            container.ChangeState("GalaxyState");
        }

        public override void OpenPlanetMenu (Command command) {
            OpenPlanetMenu();
            container.Enqueue(command);
        }

        public override void OpenPlanetMenu () {
            container.ChangeState("PlanetMenuState");
        }

        public override string ToString () {
            return "在行星系";
        }
    }

}
