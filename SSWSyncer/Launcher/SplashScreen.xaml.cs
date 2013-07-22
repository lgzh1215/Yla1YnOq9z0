using System;
using System.Windows;
using Common.Logging;
using SSWSyncer.Launcher.AppUpdater.Wpf;

namespace SSWSyncer.Launcher {
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window {

        private static ILog log = LogManager.GetLogger(typeof(SplashScreen));

        public SplashScreen () {
            log.Debug("--------------------");
            log.Debug("SSWSyncer Starting...");
            try {
                InitializeComponent();
                this.Show();
                MessageListener.Instance.ReceiveMessage("Getting update info...");
                startSSWSyncer();
                //Updater updater = getUpdater();
                //if (!updater.UpdaterArgs.ContainsValue("phase2")
                //    && !updater.UpdateIsAvailableAndValid) {
                //    log.Debug("NoUpdate Start AVSyncer.");
                //    startSSWSyncer();
                //}
            } catch (Exception e) {
                Console.Write(e.StackTrace);
                startSSWSyncer();
            }
        }

        private void startSSWSyncer () {
            MainWindow main = new MainWindow();
            MessageListener.Instance.ReceiveMessage("Ready, enjoy it!");
            System.Threading.Thread.Sleep(300);
            this.Hide();
            main.Show();
        }

        private Updater getUpdater () {
            String _manifestUrl = "http://ghchen.dlinkddns.com/SSWSyncer/Manifest.xml";
            String _infoUrl = "http://ghchen.dlinkddns.com/SSWSyncer/publish.htm";
            String _publicKeyXml = "<RSAKeyValue><Modulus>x/JTx0clHlQPtDN3ho2W0f2yFWpVl/i493XIrciYthPqz08c/wcJ4qR7jiQmw12MwyjC/mRHuwIw3Qp/W1MgbW4+UH/OLqTp9QgSgH3WVEKTl37pNYy9c/NFOH9x0kY3g8bjJdPHyXZsEjqsm3TGzjNCFfujMqGhxu2IZf9sRCs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            return new Updater(_infoUrl, _manifestUrl, _publicKeyXml);
        }
    }
}

