using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SSWSyncer.Commands {

    [Serializable]
    class AirborneCommand : AbstractCommand {

        public int Amount { get; private set; }

        public Point Point { get; set; }

        public int Formation { get; private set; }

        public AirborneCommand (int amount, Point point, int formation) {
            Amount = amount;
            Point = point;
            Formation = formation;
        }

        public AirborneCommand (Dictionary<string, object> context) {
            ParserForm(context);
        }

        public override void Update (Dictionary<string, object> context) {
            ParserForm(context);
        }

        private void ParserForm (Dictionary<string, object> context) {
            TextBox txtAmount = context["Amount"] as TextBox;
            Amount = Convert.ToInt32(txtAmount.Text);
            TextBox txtFormation = context["Formation"] as TextBox;
            Formation = Convert.ToInt32(txtFormation.Text);
            TextBox txtPlanetaryX = context["PointX"] as TextBox;
            TextBox txtPlanetaryY = context["PointY"] as TextBox;
            Point point = new Point(Convert.ToInt32(txtPlanetaryX.Text), Convert.ToInt32(txtPlanetaryY.Text));
            Point = point;
        }

        public override void Invoke (bool isSimulate, bool async) {
            OpenCommanderPanelCommand openCommanderPanelCommand = new OpenCommanderPanelCommand();
            openCommanderPanelCommand.StateContainer = StateContainer;
            openCommanderPanelCommand.Invoke(isSimulate, async);

            RecruitCommand recruitCommand = new RecruitCommand();
            recruitCommand.StateContainer = StateContainer;
            recruitCommand.Invoke(isSimulate, async);

            CloseCommanderPanelCommand closeCommanderPanelCommand = new CloseCommanderPanelCommand();
            closeCommanderPanelCommand.StateContainer = StateContainer;
            closeCommanderPanelCommand.Invoke(isSimulate, async);

            StateContainer.EnterGalaxy();
            Point M874 = new Point(100, 100);
            EnterPlanetaryCommand enterPlanetaryCommand = new EnterPlanetaryCommand(M874);
            enterPlanetaryCommand.StateContainer = StateContainer;
            enterPlanetaryCommand.Invoke(isSimulate, async);

            log.Debug(this.ToString());
            if (isSimulate) {
                // 874 III
                sim.Mouse.MoveMouseTo(256 * xf, 704 * yf).Sleep(100).LeftButtonClick().Sleep(2000);
                // 集中資源
                sim.Mouse.MoveMouseTo(306 * xf, 687 * yf).Sleep(100).LeftButtonClick().Sleep(2000);
                // 創艦隊
                sim.Mouse.MoveMouseTo(990 * xf, 654 * yf).Sleep(100).LeftButtonClick().Sleep(1500);
                if (Formation == 1) {
                    // 第一種船
                    sim.Mouse.MoveMouseTo(200 * xf, (260 + (1 * 34) - 34) * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    clikcNCell(1);
                    //// 第二種船
                    //sim.Mouse.MoveMouseTo(200 * xf, (260 + (2 * 34) - 34) * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    //clikcNCell(2);

                    //// 第三種船
                    //sim.Mouse.MoveMouseTo(200 * xf, (260 + (3 * 34) - 34) * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    //clikcNCell(3);
                } else if (Formation == 2) {
                    clikcNCell(1);
                    clikcNCell(2);
                    clikcNCell(3);
                    clikcNCell(4);
                    clikcNCell(5);
                    clikcNCell(6);
                    clikcNCell(7);
                    clikcNCell(8);
                    clikcNCell(9);
                }

                // 艦隊名
                sim.Mouse.MoveMouseTo(380 * xf, 210 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                sim.Keyboard.TextEntry("Airborne").Sleep(100);
                // 下一步
                sim.Mouse.MoveMouseTo(570 * xf, 670 * yf).Sleep(100).LeftButtonClick().Sleep(1500);
                // 大功告成
                sim.Mouse.MoveMouseTo(566 * xf, 675 * yf).Sleep(100).LeftButtonClick().Sleep(5000);
                // 最大值
                sim.Mouse.MoveMouseTo(320 * xf, 530 * yf).Sleep(100).LeftButtonClick().Sleep(1000);
                // 裝貨
                sim.Mouse.MoveMouseTo(420 * xf, 550 * yf).Sleep(100).LeftButtonClick().Sleep(1000);
                // 關閉
                sim.Mouse.MoveMouseTo(661 * xf, 646 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                // safty close
                sim.Mouse.MoveMouseTo(600 * xf, 495 * yf).Sleep(100).LeftButtonClick().Sleep(2000);
                // 移動
                sim.Mouse.MoveMouseTo(915 * xf, 405 * yf).Sleep(100).LeftButtonClick().Sleep(750);
                // 星門
                sim.Mouse.MoveMouseTo(700 * xf, 700 * yf).Sleep(100).LeftButtonClick().Sleep(1000);

                StateContainer.EnterGalaxy();
                GalaxyMovingCommand galaxyMovingCommand = new GalaxyMovingCommand(Point);
                galaxyMovingCommand.StateContainer = StateContainer;
                galaxyMovingCommand.Invoke(isSimulate, async);
                sim.Mouse.LeftButtonClick().Sleep(500);
                // 躍遷
                sim.Mouse.MoveMouseTo(870 * xf, 240 * yf).Sleep(100);
            }
        }

        public override string ToString () {
            return "從874空降" + Amount + "戰艦到" + Point;
        }

        private void clikcNCell (int i) {
            int x = (i - 1) % 3;
            int y = (i - 1) / 3;
            int px = (x * 92) + 319;
            int py = (y * 50) + 524;
            sim.Mouse.MoveMouseTo(px * xf, py * yf).Sleep(100).LeftButtonClick().Sleep(300);
            // 服役
            sim.Mouse.MoveMouseTo(370 * xf, 470 * yf).Sleep(100).LeftButtonClick().Sleep(300);
            // 輸入數量
            sim.Keyboard.TextEntry(Amount.ToString()).Sleep(100);
            // [確定]
            sim.Mouse.MoveMouseTo(525 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(300);
        }
    }

}
