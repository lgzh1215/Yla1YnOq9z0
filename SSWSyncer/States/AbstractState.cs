using System;
using SSWSyncer.Commands;

namespace SSWSyncer.States {

    abstract class AbstractState : State {

        protected StateContainer container;

        public AbstractState (StateContainer container) {
            this.container = container;
        }

        public virtual void Login (Command command) {
            throw new NotSupportedException();
        }

        public virtual void Login () {
            throw new NotSupportedException();
        }

        public virtual void Logout (Command command) {
            throw new NotSupportedException();
        }

        public virtual void Logout () {
            throw new NotSupportedException();
        }

        public virtual void EnterGalaxy (Command command) {
            throw new NotSupportedException();
        }

        public virtual void EnterGalaxy () {
            throw new NotSupportedException();
        }

        public virtual void EnterPlanetary (Command command) {
            throw new NotSupportedException();
        }

        public virtual void EnterPlanetary () {
            throw new NotSupportedException();
        }

        public virtual void EnterCommanderPanel (Command command) {
            throw new NotSupportedException();
        }

        public virtual void EnterCommanderPanel () {
            throw new NotSupportedException();
        }

        public virtual void EnterMissionPanel (Command command) {
            throw new NotSupportedException();
        }

        public virtual void EnterMissionPanel () {
            throw new NotSupportedException();
        }

        public virtual void OpenPlanetMenu (Command command) {
            throw new NotSupportedException();
        }

        public virtual void OpenPlanetMenu () {
            throw new NotSupportedException();
        }

        public virtual void EnterPlanet () {
            throw new NotSupportedException();
        }

        public virtual void EnterPlanet (Command command) {
            throw new NotSupportedException();
        }

        public virtual void EnterResourceBuildingList () {
            throw new NotSupportedException();
        }

        public virtual void EnterResourceBuildingList (Command command) {
            throw new NotSupportedException();
        }
    }
}
