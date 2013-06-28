using System;
using System.Windows;
using Launcher.AppUpdater.Wpf;
using AVSyncer;
using AVSyncerPlugin.Core;

namespace Launcher {
	/// <summary>
	/// Interaction logic for SplashScreen.xaml
	/// </summary>
	public partial class SplashScreen:Window {
		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(SplashScreen));

		public SplashScreen () {
			LogHelper.InitializeLog4NET();
			log.Debug("--------------------");
			log.Debug("AVSyncer Starting...");
			try {
				InitializeComponent();
				this.Show();
				MessageListener.Instance.ReceiveMessage("Getting update info...");
				Updater updater = getUpdater();
				if (!updater.UpdaterArgs.ContainsValue("phase2")
					&& !updater.UpdateIsAvailableAndValid) {
						log.Debug("NoUpdate Start AVSyncer.");
						startAVSyncer();
				}
			} catch (Exception e) {
				Console.Write(e.StackTrace);
				startAVSyncer();
			}
		}

		private void startAVSyncer () {
			MainWindow avs = new MainWindow();
			MessageListener.Instance.ReceiveMessage("Ready, enjoy it!");
			System.Threading.Thread.Sleep(300);
			this.Hide();
			avs.Show();
		}

		private Updater getUpdater () {
			String _manifestUrl = "http://ghchen.twbbs.org/AVSyncer/Manifest.xml";
			String _infoUrl = "http://ghchen.twbbs.org/AVSyncer/publish.htm";
			String _publicKeyXml = "<RSAKeyValue><Modulus>x/JTx0clHlQPtDN3ho2W0f2yFWpVl/i493XIrciYthPqz08c/wcJ4qR7jiQmw12MwyjC/mRHuwIw3Qp/W1MgbW4+UH/OLqTp9QgSgH3WVEKTl37pNYy9c/NFOH9x0kY3g8bjJdPHyXZsEjqsm3TGzjNCFfujMqGhxu2IZf9sRCs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
			return new Updater(_infoUrl, _manifestUrl, _publicKeyXml);
		}
	}
}

