using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Threading;
using Fiddler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;

namespace KanColleTool {
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window {

        Thread UIThread;

        DashBoard dashBoard;

        ShipListPage shipListPage;

        ItemListPage itemListPage;

        public delegate void Invoker ();

        public MainWindow () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();

            dashBoard = new DashBoard();
            shipListPage = new ShipListPage();
            itemListPage = new ItemListPage();
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
                default:
                    break;
            }
        }

        private void button1_Click (object sender, RoutedEventArgs e) {
            Assembly assembly = typeof(MainWindow).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.JSON.ship2.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                JToken temp = JToken.Parse(json);
                KCODt.Instance.OnShipDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
            }
            
        }

        private void button2_Click (object sender, RoutedEventArgs e) {
            Assembly assembly = typeof(MainWindow).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("KanColleTool.JSON.slotitem2.json"))
            using (StreamReader reader = new StreamReader(stream)) {
                string json = reader.ReadToEnd();
                JToken temp = JToken.Parse(json);
                KCODt.Instance.OnItemDataChangedEvent(new DataChangedEventArgs(temp["api_data"]));
            }
        }

    }
}
