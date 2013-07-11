using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using Common.Logging;
using Quartz;
using SSWSyncer.Commands;

namespace SSWSyncer.Core {

    [Serializable]
    public class SSWScript : IJob {
        
        private static ILog log = LogManager.GetLogger(typeof(SSWScript));

        public string InitState { get; set; }

        public ObservableCollection<Command> Commands { get; set; }

        public SSWScript() {
        }

        public SSWScript (string initState, ObservableCollection<Command> commands) {
            InitState = initState;
            Commands = commands;
        }

        public virtual void Execute (IJobExecutionContext context) {
            log.Info("Quartz execute ssw job...");
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            SSWScript sswScript = (SSWScript) dataMap["SSWScript"];
            MainWindow mainWindow = (MainWindow) dataMap["MainWindow"];
            Thread UIThread = (Thread)dataMap["UIThread"];
            if (sswScript != null && mainWindow != null) {
                Dispatcher.FromThread(UIThread).Invoke((MainWindow.SSWScriptInvoker)delegate
                {
                    mainWindow.LoadScript(sswScript);
                    mainWindow.VerifyScript();
                    mainWindow.InvokeScript(true);
                }, null);

            }
        }

    }

}
