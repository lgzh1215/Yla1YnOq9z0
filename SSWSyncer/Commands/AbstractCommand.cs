using System;
using System.Collections.Generic;
using Common.Logging;
using SSWSyncer.States;
using WindowsInput;

namespace SSWSyncer.Commands {

    [Serializable]
    abstract class AbstractCommand : Command {

        [NonSerialized]
        private StateContainer stateContainer;

        public StateContainer StateContainer {
            get { return stateContainer; }
            set { stateContainer = value; }
        }

        [NonSerialized]
        static protected ILog log = LogManager.GetLogger(typeof(Command));

        [NonSerialized]
        static protected double xf = 65535 / System.Windows.SystemParameters.PrimaryScreenWidth;

        [NonSerialized]
        static protected double yf = 65535 / System.Windows.SystemParameters.PrimaryScreenHeight;

        [NonSerialized]
        static protected InputSimulator sim = new InputSimulator();

        public virtual void Update (Dictionary<string, object> context) {
            throw new NotImplementedException();
        }

        public virtual void Invoke (bool isSimulate) {
            throw new NotImplementedException();
        }

    }

}
