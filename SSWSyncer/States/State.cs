using SSWSyncer.Commands;

namespace SSWSyncer.States {

    public interface State {

        void Login (Command command);

        void Login ();

        void Logout (Command command);

        void Logout ();

        void EnterGalaxy (Command command);

        void EnterGalaxy ();

        void EnterPlanetary (Command command);

        void EnterPlanetary ();

        void EnterCommanderPanel (Command command);

        void EnterCommanderPanel ();

        void EnterMissionPanel (Command command);

        void EnterMissionPanel ();

        void OpenPlanetMenu (Command command);

        void OpenPlanetMenu ();

        void EnterPlanet ();

        void EnterPlanet (Command command);

        void EnterResourceBuildingList ();

        void EnterResourceBuildingList (Command command);
    }
}
