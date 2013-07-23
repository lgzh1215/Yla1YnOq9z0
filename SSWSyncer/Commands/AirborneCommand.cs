using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SSWSyncer.Commands {

    [Serializable]
    class AirborneCommand : AbstractCommand {

        public int Amount { get; private set; }

        public Point Point { get; set; }

        public AirborneCommand (int amount, Point point) {
            Amount = amount;
            Point = point;
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
            TextBox txtPlanetaryX = context["PointX"] as TextBox;
            TextBox txtPlanetaryY = context["PointY"] as TextBox;
            Point point = new Point(Convert.ToInt32(txtPlanetaryX.Text), Convert.ToInt32(txtPlanetaryY.Text));
            Point = point;
        }

        public override void Invoke (bool isSimulate, bool async) {
            StateContainer.EnterGalaxy();
            Point M874 = new Point(100, 100);
            EnterPlanetaryCommand enterPlanetaryCommand = new EnterPlanetaryCommand(M874);
            enterPlanetaryCommand.StateContainer = StateContainer;
            enterPlanetaryCommand.Invoke(isSimulate, async);

            log.Debug(this.ToString());
            if (isSimulate) {
                // 創艦隊
                sim.Mouse.MoveMouseTo(990 * xf, 654 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                
                // 第一種船
                sim.Mouse.MoveMouseTo(200 * xf, (260 + (1 * 34) - 34) * yf).Sleep(100).LeftButtonClick().Sleep(300);
                // 第一格
                sim.Mouse.MoveMouseTo(300 * xf, 520 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                // 服役
                sim.Mouse.MoveMouseTo(370 * xf, 470 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                // 輸入數量
                sim.Keyboard.TextEntry(Amount.ToString()).Sleep(100);
                // [確定]
                sim.Mouse.MoveMouseTo(525 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(300);

                // 第二種船
                sim.Mouse.MoveMouseTo(200 * xf, (260 + (2 * 34) - 34) * yf).Sleep(100).LeftButtonClick().Sleep(300);
                // 第二格
                sim.Mouse.MoveMouseTo(385 * xf, 520 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                // 服役
                sim.Mouse.MoveMouseTo(370 * xf, 470 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                // 輸入數量
                sim.Keyboard.TextEntry(Amount.ToString()).Sleep(100);
                // [確定]
                sim.Mouse.MoveMouseTo(525 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                
                // 第三種船
                sim.Mouse.MoveMouseTo(200 * xf, (260 + (3 * 34) - 34) * yf).Sleep(100).LeftButtonClick().Sleep(300);
                // 第三格
                sim.Mouse.MoveMouseTo(480 * xf, 520 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                // 服役
                sim.Mouse.MoveMouseTo(370 * xf, 470 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                // 輸入數量
                sim.Keyboard.TextEntry(Amount.ToString()).Sleep(100);
                // [確定]
                sim.Mouse.MoveMouseTo(525 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                
                // 艦隊名
                sim.Mouse.MoveMouseTo(380 * xf, 210 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                sim.Keyboard.TextEntry("Airborne").Sleep(100);
                // 下一步
                sim.Mouse.MoveMouseTo(570 * xf, 670 * yf).Sleep(100).LeftButtonClick().Sleep(1500);
                // 大功告成
                sim.Mouse.MoveMouseTo(566 * xf, 675 * yf).Sleep(100).LeftButtonClick().Sleep(2000);
                // 最大值
                sim.Mouse.MoveMouseTo(320 * xf, 530 * yf).Sleep(100).LeftButtonClick().Sleep(1000);
                // 裝貨
                sim.Mouse.MoveMouseTo(420 * xf, 550 * yf).Sleep(100).LeftButtonClick().Sleep(1000);
                // 關閉
                sim.Mouse.MoveMouseTo(661 * xf, 646 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                // 移動
                // sim.Mouse.MoveMouseTo(?? * xf, ?? * yf).Sleep(100).LeftButtonClick().Sleep(1000);

                GalaxyMovingCommand galaxyMovingCommand = new GalaxyMovingCommand(Point);
                galaxyMovingCommand.StateContainer = StateContainer;
                galaxyMovingCommand.Invoke(isSimulate, async);

                // mousemove to tgt point n click
                // 躍遷
            }
        }

        public override string ToString () {
            return "從874空降" + Amount +"戰艦到" + Point;
        }

    }

}
