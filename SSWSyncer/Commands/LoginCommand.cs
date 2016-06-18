using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SSWSyncer.Core;
using WindowsInput.Native;

namespace SSWSyncer.Commands {

    [Serializable]
    class LoginCommand : AbstractCommand {

        private static BitmapImage loginProof;

        public UserInfo UserInfo { get; set; }

        public LoginCommand (UserInfo userInfo) {
            UserInfo = userInfo;
        }

        public LoginCommand (Dictionary<string, object> context) {
            ParserForm(context);
        }

        public override void Update (Dictionary<string, object> context) {
            ParserForm(context);
        }

        private void ParserForm (Dictionary<string, object> context) {
            ComboBox Combobox = context["UserInfo"] as ComboBox;
            UserInfo selected = Combobox.SelectedItem as UserInfo;
            UserInfo = selected;
        }

        public override void Invoke (bool isSimulate, bool async) {
            log.Debug(this.ToString());
            bool saveScreen;
            Boolean.TryParse(StateContainer.ini.Read("saveScreen", "Settings"), out saveScreen);
            Uri proofImg = new Uri(Environment.CurrentDirectory + @"\proofImg\login.bmp");
            loginProof = new BitmapImage(proofImg);
            StateContainer.Login();
            int tries = 0;
            bool loading = false;
            bool signIn = false;
            Bitmap bmpScreenshot;
            Graphics gfx;
            byte Luminosity;
            if (isSimulate) {
                while (!loading) {
                    if (!signIn) {
                        changeMac();
                        doLogin();
                        signIn = true;
                    }
                    tries++;
                    if (tries > 30) {
                        break;
                    }
                    sim.Mouse.Sleep(4000);
                    bmpScreenshot = new Bitmap(145, 23, PixelFormat.Format32bppArgb);
                    gfx = Graphics.FromImage(bmpScreenshot);
                    gfx.CopyFromScreen(32, 32, 0, 0,
                                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size,
                                CopyPixelOperation.SourceCopy);
                    loading = Utils.Compare(Utils.BitmapImage2Bitmap(loginProof), bmpScreenshot);
                    log.Debug("Loading...");
                    if (loading) {
                        break;
                    } else if (saveScreen) {
                        Utils.saveScreen(Environment.CurrentDirectory + @"\loginTry", bmpScreenshot);
                    }
                    bmpScreenshot = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
                    gfx = Graphics.FromImage(bmpScreenshot);
                    gfx.CopyFromScreen(590, 490, 0, 0,
                                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size,
                                CopyPixelOperation.SourceCopy);
                    Color color = bmpScreenshot.GetPixel(0, 0);
                    Luminosity = (byte) (color.GetBrightness() * 255);
                    log.Debug("signIn color.Lum:" + Luminosity);
                    if (Luminosity < 150 && Luminosity > 140) {
                        // create user
                        sim.Mouse.MoveMouseTo(550 * xf, 345 * yf).Sleep(250).LeftButtonClick().Sleep(500);
                        sim.Keyboard.TextEntry(UserInfo.Name).Sleep(500);
                        sim.Mouse.MoveMouseTo(465 * xf, 522 * yf).Sleep(250).LeftButtonClick();
                    }
                    if (Luminosity < 120) {
                        sim.Mouse.MoveMouseTo(590 * xf, 490 * yf).Sleep(250).LeftButtonClick();
                        signIn = false;
                        continue;
                    }
                }
            }
        }

        public override string ToString () {
            return "登入:" + UserInfo;
        }

        private void doLogin () {
            sim.Mouse.MoveMouseTo(100 * xf, 100 * yf).Sleep(250).LeftButtonClick().Sleep(250);
            sim.Mouse.MoveMouseTo(590 * xf, 580 * yf).Sleep(250).LeftButtonClick().Sleep(250);
            sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A).Sleep(500);
            sim.Keyboard.TextEntry(UserInfo.Account).Sleep(500);
            sim.Mouse.MoveMouseTo(480 * xf, 610 * yf).Sleep(250).LeftButtonClick().Sleep(500);
            sim.Keyboard.TextEntry(UserInfo.Password).Sleep(500);
            sim.Mouse.MoveMouseTo(600 * xf, 644 * yf).Sleep(250).LeftButtonClick();
        }

        private void changeMac () {
            bool enabled;
            Boolean.TryParse(StateContainer.ini.Read("changeMac", "Settings"), out enabled);
            if (!enabled) {
                return;
            }
            try {
                log.Debug("Change MAC Address");
                string netCfgInstanceId = StateContainer.nic.Id;
                string newmac = generateMACAddress();
                string baseReg = @"SYSTEM\CurrentControlSet\Control\Class\{4D36E972-E325-11CE-BFC1-08002bE10318}\";
                string query = "SELECT * FROM Win32_NetworkAdapter where GUID = '" + netCfgInstanceId + "'";
                RegistryKey subKey = null;
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
                using (RegistryKey midKey = baseKey.OpenSubKey(baseReg)) {
                    foreach (string sub in midKey.GetSubKeyNames()) {
                        try {
                            subKey = midKey.OpenSubKey(sub, RegistryKeyPermissionCheck.ReadWriteSubTree);
                            string x = subKey.GetValue("NetCfgInstanceId", "") as string;
                            if (netCfgInstanceId.Equals(x)) {
                                log.Debug(sub + ": " + x.ToString());
                                log.Debug("New MAC Address: " + newmac);
                                subKey.SetValue("NetworkAddress", newmac, RegistryValueKind.String);
                                break;
                            }
                        } catch (Exception e) {
                            log.Debug(e.StackTrace);
                        }
                    }
                    if (subKey != null) {
                        ManagementObjectSearcher mos = new ManagementObjectSearcher(new SelectQuery(query));
                        foreach (ManagementObject o in mos.Get().OfType<ManagementObject>()) {
                            log.Debug("Network Disable");
                            o.InvokeMethod("Disable", null);
                            log.Debug("Network Enable");
                            o.InvokeMethod("Enable", null);
                            sim.Mouse.Sleep(10000);
                        }
                    }
                }
            } catch (Exception e) {
                log.Debug(e.StackTrace);
            }
        }

        private string generateMACAddress () {
            var sBuilder = new StringBuilder();
            var r = new Random();
            int number;
            byte b;
            for (int i = 0; i < 6; i++) {
                number = r.Next(0, 255);
                b = Convert.ToByte(number);
                if (i == 0) {
                    b = setBit(b, 6); //--> set locally administered
                    b = unsetBit(b, 7); // --> set unicast 
                }
                sBuilder.Append(number.ToString("X2"));
            }
            return sBuilder.ToString().ToUpper();
        }

        private byte setBit (byte b, int BitNumber) {
            if (BitNumber < 8 && BitNumber > -1) {
                return (byte) (b | (byte) (0x01 << BitNumber));
            } else {
                throw new InvalidOperationException(
                "Der Wert für BitNumber " + BitNumber.ToString() + " war nicht im zulässigen Bereich! (BitNumber = (min)0 - (max)7)");
            }
        }

        private byte unsetBit (byte b, int BitNumber) {
            if (BitNumber < 8 && BitNumber > -1) {
                return (byte) (b | (byte) (0x00 << BitNumber));
            } else {
                throw new InvalidOperationException(
                "Der Wert für BitNumber " + BitNumber.ToString() + " war nicht im zulässigen Bereich! (BitNumber = (min)0 - (max)7)");
            }
        }
    }

}
