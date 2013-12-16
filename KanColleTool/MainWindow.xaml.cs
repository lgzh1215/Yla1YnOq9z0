using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace KanColleTool {
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window {

        Thread UIThread;

        DashBoard dashBoard;

        ShipListPage shipListPage;

        ItemListPage itemListPage;

        QuestListPage questListPage;

        public delegate void Invoker ();

        public MainWindow () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();

            dashBoard = new DashBoard();
            shipListPage = new ShipListPage();
            itemListPage = new ItemListPage();
            questListPage = new QuestListPage();
            mainFrame.NavigationService.Navigate(dashBoard);
        }

        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            Debug.Print("Shutting down...");
            Fiddler.FiddlerApplication.Shutdown();
            Thread.Sleep(500);
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void Button_Click (object sender, RoutedEventArgs e) {
            Button btn = (Button) sender;
            switch (btn.Uid) {
                case "DashBoard":
                    mainFrame.NavigationService.Navigate(dashBoard);
                    break;
                case "ShipListPage":
                    mainFrame.NavigationService.Navigate(shipListPage);
                    break;
                case "ItemListPage":
                    mainFrame.NavigationService.Navigate(itemListPage);
                    break;
                case "QuestListPage":
                    mainFrame.NavigationService.Navigate(questListPage);
                    break;
                default:
                    break;
            }
        }

    }
}
