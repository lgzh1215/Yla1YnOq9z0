using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using SSWSyncer.Commands;
using SSWSyncer.Core;
using SSWSyncer.States;

namespace SSWSyncer {
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window {

        StateContainer script = new StateContainer();

        List<UserInfo> users = new List<UserInfo>();

        ObservableCollection<Command> ListItems = new ObservableCollection<Command> { };

        ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

        IScheduler scheduler;

        private static ILog log = LogManager.GetLogger(typeof(MainWindow));

        public MainWindow () {
            InitializeComponent();
            listBox1.ItemsSource = ListItems;
            initScheduler();
            foreach (string key in script.StateMap.Keys) {
                cmbInitState.Items.Add(script.StateMap[key]);
            }
            cmbInitState.SelectedIndex = 0;
            users.Add(new UserInfo("chen.guan.hwa@gmail.com", "1qaz@WSX"));
            users.Add(new UserInfo("76450106@qq.com", "5462606"));
            users.Add(new UserInfo("cwc12345@gmail.com", "cwc12345"));
            users.Add(new UserInfo("cwc54321@gmail.com", "cwc12345"));
            users.Add(new UserInfo("sanakan0202@yahoo.com.tw", "74107410"));
            users.Add(new UserInfo("sephiroth8571@hotmail.com", "955593597"));
            users.Add(new UserInfo("hd0001@hd.com", "123456"));
            users.Add(new UserInfo("hd0002@hd.com", "123456"));
            users.Add(new UserInfo("hd0003@hd.com", "123456"));
            users.Add(new UserInfo("kin8591@hotmail.com", "livelihooh"));
            users.Add(new UserInfo("qazwsx@yahoo.com", "qazwsxedcrfv"));
            users.Add(new UserInfo("cwc12345cwc@gmail.com", "cwc12345"));
            users.Add(new UserInfo("weijack1524@hotmail.com", "jack1985"));
            users.Add(new UserInfo("76550001@qq.com", "123456"));
            users.Add(new UserInfo("76550002@qq.com", "123456"));
            users.Add(new UserInfo("s109@qq.com", "123123"));
            users.Add(new UserInfo("s110@qq.com", "123123"));
            users.Add(new UserInfo("s111@qq.com", "123123"));
            users.Add(new UserInfo("s112@qq.com", "123123"));
            users.Add(new UserInfo("s113@qq.com", "123123"));
            users.Add(new UserInfo("s114@qq.com", "123123"));
            users.Add(new UserInfo("s115@qq.com", "123123"));
            users.Add(new UserInfo("s116@qq.com", "123123"));
            users.Add(new UserInfo("s117@qq.com", "123123"));
            users.Add(new UserInfo("s118@qq.com", "123123"));
            users.Add(new UserInfo("s119@qq.com", "123123"));
            users.Add(new UserInfo("s120@qq.com", "123123"));
            users.Add(new UserInfo("s121@qq.com", "123123"));
        }

        public delegate void SSWScriptInvoker ();

        public void LoadScript (SSWScript sswScript) {
            listBox1.ItemsSource = null;
            ListItems.Clear();
            ListItems = sswScript.Commands;
            listBox1.ItemsSource = ListItems;
            cmbInitState.SelectedValue = script.StateMap[sswScript.InitState];
        }

        public void VerifyScript () {
            ClearCommandGrid();
            if (chkMultiUser.IsChecked == true) {
                //foreach (LoginCommand aLogin in users) {
                //    script.Clear();
                //    script.ChangeState("NotLoggedinState");
                //    script.Enqueue(aLogin);
                //    foreach (Command command in ListItems) {
                //        script.Enqueue(command);
                //    }
                //    script.Enqueue(new LogoutCommand());
                //    try {
                //        script.Invoke(false);
                //    } catch (Exception ex) {
                //        log.info(ex.Message);
                //    }
                //}
            } else {
                script.Clear();
                script.ChangeState(cmbInitState.SelectedItem.GetType().Name);
                foreach (Command command in ListItems) {
                    script.Enqueue(command);
                }
                try {
                    script.Invoke(false);
                } catch (NotSupportedCommandException ex) {
                    log.Error(ex.Message);
                    listBox1.SelectedItem = ex.Command;
                    listBox1.Focus();
                }
            }
        }

        public void InvokeScript () {
            ClearCommandGrid();
            if (chkMultiUser.IsChecked == true) {
                //foreach (LoginCommand aLogin in users) {
                //    script.Clear();
                //    script.ChangeState("NotLoggedinState");
                //    script.Enqueue(aLogin);
                //    foreach (Command command in ListItems) {
                //        script.Enqueue(command);
                //    }
                //    script.Enqueue(new LogoutCommand());
                //    script.Invoke(true);
                //}
            } else {
                script.Clear();
                script.ChangeState(cmbInitState.SelectedItem.GetType().Name);
                foreach (Command command in ListItems) {
                    script.Enqueue(command);
                }
                script.Invoke(true);
            }
        }

        private void initScheduler () {
            scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();
        }

        #region ScriptFunction
        private void btnVerify_Click (object sender, RoutedEventArgs e) {
            VerifyScript();
        }

        private void btnInvoke_Click (object sender, RoutedEventArgs e) {
            InvokeScript();
        }

        private void btnClear_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            ListItems.Clear();
            script.Clear();
            script.ChangeState("NotLoggedinState");
        }

        private void btnDeleteCommand_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            int index = listBox1.Items.IndexOf(listBox1.SelectedItem);
            if (index >= 0) {
                ListItems.RemoveAt(index);
            }
        }

        private void btnMoveUp_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            int index = listBox1.SelectedIndex;
            if (index > 0) {
                var itemToMoveUp = ListItems[index];
                ListItems.RemoveAt(index);
                ListItems.Insert(index - 1, itemToMoveUp);
                listBox1.SelectedIndex = index - 1;
            }
        }

        private void btnMoveDown_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            var index = listBox1.SelectedIndex;
            if (index + 1 < ListItems.Count) {
                var itemToMoveDown = ListItems[index];
                ListItems.RemoveAt(index);
                ListItems.Insert(index + 1, itemToMoveDown);
                listBox1.SelectedIndex = index + 1;
            }
        }

        private void btnSave_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "SSW Script Files (*.ssw)|*.ssw|All Files(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog.FileName != "") {
                try {
                    using (Stream output = File.Create(saveFileDialog.FileName)) {
                        BinaryFormatter bf = new BinaryFormatter();
                        SSWScript sswScript = new SSWScript(cmbInitState.SelectedItem.GetType().Name, ListItems);
                        bf.Serialize(output, sswScript);
                    }
                } catch (Exception) {
                    MessageBox.Show("無法預期之錯誤");
                }
            }
        }

        private void btnLoad_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "SSW Script Files (*.ssw)|*.ssw|All Files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && openFileDialog.FileName != "") {
                try {
                    using (Stream input = File.OpenRead(openFileDialog.FileName)) {
                        BinaryFormatter bf = new BinaryFormatter();
                        SSWScript sswScript = (SSWScript) bf.Deserialize(input);
                        LoadScript(sswScript);
                    }
                } catch (Exception) {
                    MessageBox.Show("無法辨識");
                }
            }
        }

        private void btnQuartz_Click (object sender, RoutedEventArgs e) {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "SSW Script Files (*.ssw)|*.ssw|All Files(*.*)|*.*";
            SSWScript sswScript = null;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && openFileDialog.FileName != "") {
                try {
                    using (Stream input = File.OpenRead(openFileDialog.FileName)) {
                        BinaryFormatter bf = new BinaryFormatter();
                        sswScript = (SSWScript) bf.Deserialize(input);
                    }
                } catch (Exception) {
                    MessageBox.Show("無法辨識");
                }
                try {
                    log.Info("scheduler");
                    IJobDetail job = JobBuilder.Create<SSWScript>().WithIdentity("job1", "group1").Build();
                    DateTimeOffset runTime = DateBuilder.EvenMinuteDate(DateTimeOffset.UtcNow);
                    ITrigger trigger = TriggerBuilder.Create().WithIdentity("trigger1", "group1").StartAt(runTime).Build();
                    job.JobDataMap["SSWScript"] = sswScript;
                    job.JobDataMap["MainWindow"] = this;
                    job.JobDataMap["UIThread"] = Thread.CurrentThread;
                    scheduler.ScheduleJob(job, trigger);

                } catch (Exception ex) {
                    MessageBox.Show("Quartz error:" + ex.Message);
                }
            }
        }
        #endregion


        #region CommandDelegate
        private void btnRecruit_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            ListItems.Add(new OpenCommanderPanelCommand());
            ListItems.Add(new RecruitCommand());
            ListItems.Add(new CloseCommanderPanelCommand());
        }

        private void btnOpenMission_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            ListItems.Add(new OpenMissionPanelCommand());
        }

        private void btnCloseMission_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            ListItems.Add(new CloseMissionPanelCommand());
        }

        private void btnEnterGalaxy_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            ListItems.Add(new EnterGalaxyCommand());
        }

        private void btnLogout_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            ListItems.Add(new LogoutCommand());
        }

        private void btnClosePlanetMenu_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            ListItems.Add(new ClosePlanetMenuCommand());
        }

        private void btnCloseFacility_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            ListItems.Add(new CloseFacilityBuildPanelCommand());
        }

        private void btnLeavePlanet_Click (object sender, RoutedEventArgs e) {
            ClearCommandGrid();
            ListItems.Add(new LeavePlanetCommand());
        }

        private void btnLogin_Click (object sender, RoutedEventArgs e) {
            CreateControls(typeof(LoginCommand));
        }

        private void btnEnterPlanetary_Click (object sender, RoutedEventArgs e) {
            CreateControls(typeof(EnterPlanetaryCommand));
        }

        private void btnHarvest_Click (object sender, RoutedEventArgs e) {
            CreateControls(typeof(HarvestMissionCommand));
        }

        private void btnOpenPlanetMenu_Click (object sender, RoutedEventArgs e) {
            CreateControls(typeof(OpenPlanetMenuCommand));
        }

        private void btnGalxyMoving_Click (object sender, RoutedEventArgs e) {
            CreateControls(typeof(GalaxyMovingCommand));
        }

        private void btnEnterPlanet_Click (object sender, RoutedEventArgs e) {
            CreateControls(typeof(EnterPlanetCommand));
        }

        private void btnFacility_Click (object sender, RoutedEventArgs e) {
            CreateControls(typeof(OpenFacilityBuildPanelCommand));
        }

        private void btnBuildFacility_Click (object sender, RoutedEventArgs e) {
            CreateControls(typeof(BuildFacilityCommand));
        }

        private void btnSleep_Click (object sender, RoutedEventArgs e) {
            CreateControls(typeof(SleepCommand));
        }

        private void btnFleet_Click (object sender, RoutedEventArgs e) {
            CreateControls(typeof(FleetCommand));
        }
        #endregion

        #region function
        private RowDefinition CreateRowDefinition () {
            RowDefinition RowDefinition = new RowDefinition();
            RowDefinition.Height = GridLength.Auto;
            return RowDefinition;
        }

        private TextBlock CreateTextBlock (string text, int row, int column) {
            TextBlock tb = new TextBlock() { Text = text, Margin = new Thickness(5) };
            tb.MinWidth = 90;
            tb.FontWeight = FontWeights.Bold;
            tb.Margin = new Thickness(5);
            var bc = new BrushConverter();
            tb.Foreground = (Brush) bc.ConvertFrom("#FF2D72BC");
            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);
            return tb;
        }

        private TextBox CreateTextBox (int row, int column) {
            TextBox tb = new TextBox();
            tb.Margin = new Thickness(5);
            tb.Width = 150;
            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);
            return tb;
        }

        private CheckBox CreateCheckBox (int row, int column) {
            CheckBox cb = new CheckBox();
            cb.Margin = new Thickness(5);
            cb.MinWidth = 50;
            Grid.SetColumn(cb, column);
            Grid.SetRow(cb, row);
            return cb;
        }

        private Button CreateButton (string text, int row, int column) {
            Button tb = new Button() {
                Content = text,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5)
            };
            tb.Width = 90;
            tb.Height = 25;
            tb.Margin = new Thickness(5);
            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);
            return tb;
        }

        private bool CreateControls (Type type) {
            return CreateControls(type, null);
        }

        private bool CreateControls (Type type, Object instance) {
            ClearCommandGrid();
            Dictionary<string, object> context = new Dictionary<string, object>();
            PropertyInfo[] propertyInfos = type.GetProperties();
            grid.RowDefinitions.Add(CreateRowDefinition());
            int j = 1;
            context.Add("Type", type);
            foreach (PropertyInfo propertyInfo in propertyInfos) {
                MethodInfo getMethod = propertyInfo.GetGetMethod();
                string typeName = propertyInfo.PropertyType.Name;
                string propertyName = propertyInfo.Name;
                if (typeName == "String" || typeName == "Int32") {
                    CreateTextRowStuff(j, propertyName, context);
                    j++;
                    if (instance != null) {
                        var value = getMethod.Invoke(instance, null);
                        (context[propertyName] as TextBox).Text = value.ToString();
                    }
                } else if (typeName == "Point") {
                    CreateTextRowStuff(j, "PointX", context);
                    j++;
                    CreateTextRowStuff(j, "PointY", context);
                    j++;
                    if (instance != null) {
                        Point point = (Point) getMethod.Invoke(instance, null);
                        (context["PointX"] as TextBox).Text = point.X.ToString();
                        (context["PointY"] as TextBox).Text = point.Y.ToString();
                    }
                } else if (typeName == "Boolean" || typeName == "bool") {
                    CreateCheckBoxRowStuff(j, propertyName, context);
                    j++;
                    if (instance != null) {
                        bool value = (bool) getMethod.Invoke(instance, null);
                        (context[propertyName] as CheckBox).IsChecked = value;
                    }
                } else if (typeName == "FacilityType") {
                    CreateComboBoxRowStuff(j, "Facility", OpenFacilityBuildPanelCommand.Facility.Values, context);
                    j++;
                    if (instance != null) {
                        FacilityType facilityType = (FacilityType) getMethod.Invoke(instance, null);
                        string selected = OpenFacilityBuildPanelCommand.Facility[facilityType];
                        (context["Facility"] as ComboBox).SelectedValue = selected;
                    }
                } else if (typeName == "UserInfo") {
                    CreateComboBoxRowStuff(j, "UserInfo", users, context);
                    j++;
                    if (instance != null) {
                        UserInfo userInfo = (UserInfo) getMethod.Invoke(instance, null);
                        (context["UserInfo"] as ComboBox).SelectedValue = userInfo;
                    }
                }
            }
            if (j != 1) {
                grid.RowDefinitions.Add(CreateRowDefinition());
                var Button = CreateButton("Save", j, 1);
                if (instance == null) {
                    Button.Click += new RoutedEventHandler(save_Click);
                } else {
                    context.Add("Instance", instance);
                    Button.Click += new RoutedEventHandler(update_Click);
                }
                Button.Tag = context;
                grid.Children.Add(Button);
                return true;
            }
            return false;
        }

        private void CreateComboBoxRowStuff (int j, string name, ICollection Collection, Dictionary<string, object> context) {
            grid.RowDefinitions.Add(CreateRowDefinition());
            var Label = CreateTextBlock(name, j, 0);
            grid.Children.Add(Label);
            ComboBox Combobox = new ComboBox();
            foreach (Object value in Collection) {
                Combobox.Items.Add(value);
            }
            Combobox.SelectedIndex = 0;
            Combobox.Margin = new Thickness(5);
            Combobox.Width = 150;
            Grid.SetColumn(Combobox, 1);
            Grid.SetRow(Combobox, j);
            grid.Children.Add(Combobox);
            context.Add(name, Combobox);
        }

        private void ClearCommandGrid () {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100.0) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        }

        private void CreateTextRowStuff (int j, string name, Dictionary<string, object> context) {
            grid.RowDefinitions.Add(CreateRowDefinition());
            var Label = CreateTextBlock(name, j, 0);
            grid.Children.Add(Label);
            var Textbox = CreateTextBox(j, 1);
            grid.Children.Add(Textbox);
            context.Add(name, Textbox);
        }

        private void CreateCheckBoxRowStuff (int j, string name, Dictionary<string, object> context) {
            grid.RowDefinitions.Add(CreateRowDefinition());
            var Label = CreateTextBlock(name, j, 0);
            grid.Children.Add(Label);
            var Checkbox = CreateCheckBox(j, 1);
            grid.Children.Add(Checkbox);
            context.Add(name, Checkbox);
        }

        private void save_Click (object sender, RoutedEventArgs e) {
            Button btn = sender as Button;
            Dictionary<string, object> context = btn.Tag as Dictionary<string, object>;
            Type type = context["Type"] as Type;
            ConstructorInfo ci = type.GetConstructor(new Type[1] { typeof(Dictionary<string, object>) });
            try {
                Command cmd = ci.Invoke(new object[] { context }) as Command;
                ListItems.Add(cmd);
            } catch (Exception ex) {
                log.Info(ex.Message);
            }
        }

        private void update_Click (object sender, RoutedEventArgs e) {
            Button btn = sender as Button;
            Dictionary<string, object> context = btn.Tag as Dictionary<string, object>;
            Type type = context["Type"] as Type;
            Command command = context["Instance"] as Command;
            MethodInfo update = type.GetMethod("Update", new Type[1] { typeof(Dictionary<string, object>) });
            try {
                update.Invoke(command, new object[] { context });
                listBox1.ItemsSource = null;
                listBox1.ItemsSource = ListItems;
            } catch (Exception ex) {
                log.Info(ex.Message);
            }
        }
        #endregion

        private void listBox1_PreviewMouseDown (object sender, System.Windows.Input.MouseButtonEventArgs e) {
            try {
                var item = ItemsControl.ContainerFromElement(listBox1, e.OriginalSource as DependencyObject) as ListBoxItem;
                CreateControls(item.Content.GetType(), item.Content);
            } catch (Exception ex) {
                log.Error(ex.Message);
            }
        }

    }

}
