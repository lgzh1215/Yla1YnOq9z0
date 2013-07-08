using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using SSWSyncer.Commands;
using Common.Logging;

namespace SSWSyncer.States {

    public class StateContainer : State, Command {

        static protected ILog log = LogManager.GetLogger(typeof(State));

        private State mainState;

        private State notLoggedinState;

        private State galaxyState;

        private State commanderState;

        private State missionState;

        private State planetaryState;

        private State planetMenuState;

        private State planetState;

        private State resourceBuildingState;

        private Dictionary<string, State> stateMap;

        private Queue<Command> tasks;

        public bool OnInvoke { get; private set; }

        public Point ExtentMin { get; set; }

        public Point ExtentMax { get; set; }

        public Dictionary<string, State> StateMap {
            get {
                return stateMap;
            }
        }

        public StateContainer () {
            // 新狀態要來這邊註冊
            notLoggedinState = new NotLoggedinState(this);
            galaxyState = new GalaxyState(this);
            commanderState = new CommanderState(this);
            missionState = new MissionState(this);
            planetaryState = new PlanetaryState(this);
            planetMenuState = new PlanetMenuState(this);
            planetState = new PlanetState(this);
            resourceBuildingState = new ResourceBuildingState(this);

            stateMap = new Dictionary<string, State>();
            stateMap.Add(notLoggedinState.GetType().Name, notLoggedinState);
            stateMap.Add(galaxyState.GetType().Name, galaxyState);
            stateMap.Add(commanderState.GetType().Name, commanderState);
            stateMap.Add(missionState.GetType().Name, missionState);
            stateMap.Add(planetaryState.GetType().Name, planetaryState);
            stateMap.Add(planetMenuState.GetType().Name, planetMenuState);
            stateMap.Add(planetState.GetType().Name, planetState);
            stateMap.Add(resourceBuildingState.GetType().Name, resourceBuildingState);

            mainState = notLoggedinState;
            tasks = new Queue<Command>();
            ExtentMin = new Point(0, 0);
            ExtentMax = new Point(790, 660);
            OnInvoke = false;
        }

        public void ChangeState (string typeName) {
            mainState = stateMap[typeName];
        }

        public void Clear () {
            tasks.Clear();
        }

        public void Enqueue (Command command) {
            AbstractCommand cmd = command as AbstractCommand;
            cmd.StateContainer = this;
            tasks.Enqueue(command);
        }

        public void Invoke (bool isSimulate) {
            if (OnInvoke) {
                log.Info("前一個腳本還在執行中");
                return;
            }
            if (isSimulate) {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(
                    delegate(object o, DoWorkEventArgs args) {
                        BackgroundWorker b = o as BackgroundWorker;
                        OnInvoke = true;
                        try {
                            while (tasks.Count > 0) {
                                Command job = tasks.Dequeue();
                                job.Invoke(isSimulate);
                            }
                        } catch (Exception) {
                        }
                    });
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                    delegate(object o, RunWorkerCompletedEventArgs args) {
                        OnInvoke = false;
                        log.Debug("指令執行完畢");
                    });
                bw.RunWorkerAsync();
            } else {
                Command job = null;
                int index = 0;
                try {
                    var len = tasks.Count + 0;
                    for (index = 0; index < len; index++) {
                        job = tasks.Dequeue();
                        job.Invoke(isSimulate);
                    }
                } catch (NotSupportedException) {
                    NotSupportedCommandException e = new NotSupportedCommandException(index, job);
                    throw e;
                }
            }
        }

        public void Login (Command command) {
            mainState.Login(command);
        }

        public void Login () {
            mainState.Login();
        }

        public void Logout (Command command) {
            mainState.Logout(command);
        }

        public void Logout () {
            mainState.Logout();
        }

        public void EnterGalaxy (Command command) {
            mainState.EnterGalaxy(command);
        }

        public void EnterGalaxy () {
            mainState.EnterGalaxy();
        }

        public void EnterPlanetary (Command command) {
            mainState.EnterPlanetary(command);
        }

        public void EnterPlanetary () {
            mainState.EnterPlanetary();
        }

        public void EnterCommanderPanel (Command command) {
            mainState.EnterCommanderPanel(command);
        }

        public void EnterCommanderPanel () {
            mainState.EnterCommanderPanel();
        }

        public void EnterMissionPanel (Command command) {
            mainState.EnterMissionPanel(command);
        }

        public void EnterMissionPanel () {
            mainState.EnterMissionPanel();
        }

        public void OpenPlanetMenu (Command command) {
            mainState.OpenPlanetMenu(command);
        }

        public void OpenPlanetMenu () {
            mainState.OpenPlanetMenu();
        }

        public void EnterPlanet (Command command) {
            mainState.EnterPlanet(command);
        }

        public void EnterPlanet () {
            mainState.EnterPlanet();
        }

        public void EnterResourceBuildingList (Command command) {
            mainState.EnterResourceBuildingList(command);
        }

        public void EnterResourceBuildingList () {
            mainState.EnterResourceBuildingList();
        }

    }
}
