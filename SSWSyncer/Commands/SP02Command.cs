using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SSWSyncer.Commands {

    [Serializable]
    class SP02Command : AbstractCommand {

        public int Stage { get; private set; }

        public SP02Command (int stage) {
            Stage = stage;
        }

        public SP02Command (Dictionary<string, object> context) {
            ParserForm(context);
        }

        public override void Update (Dictionary<string, object> context) {
            ParserForm(context);
        }

        private void ParserForm (Dictionary<string, object> context) {
            TextBox txtStage = context["Stage"] as TextBox;
            Stage = Convert.ToInt32(txtStage.Text);
        }

        public override void Invoke (bool isSimulate) {
            log.Debug(this.ToString());
            if (isSimulate) {
                if (Stage == 1) {
                    #region s1
                    // 研究
                    sim.Mouse.MoveMouseTo(850 * xf, 654 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 第一次確定
                    sim.Mouse.MoveMouseTo(608 * xf, 493 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 地質學
                    sim.Mouse.MoveMouseTo(200 * xf, 220 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    sim.Mouse.MoveMouseTo(515 * xf, 460 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // close
                    sim.Mouse.MoveMouseTo(615 * xf, 635 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    #endregion
                } else if (Stage == 2) {
                    #region s2
                    // M874II
                    sim.Mouse.MoveMouseTo(307 * xf, 443 * yf).Sleep(100).LeftButtonClick().Sleep(2000);
                    // 造船
                    sim.Mouse.MoveMouseTo(258 * xf, 420 * yf).Sleep(100).LeftButtonClick().Sleep(500);
                    // Task
                    sim.Mouse.MoveMouseTo(193 * xf, 284 * yf).Sleep(100).LeftButtonClick().Sleep(500);
                    // 確認
                    sim.Mouse.MoveMouseTo(534 * xf, 460 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 關閉
                    sim.Mouse.MoveMouseTo(610 * xf, 635 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 託管
                    sim.Mouse.MoveMouseTo(350 * xf, 450 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    #endregion
                } else if (Stage == 3) {
                    #region s3
                    // 創艦隊
                    sim.Mouse.MoveMouseTo(990 * xf, 654 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 第二種船
                    sim.Mouse.MoveMouseTo(200 * xf, (260 + (2 * 34) - 34) * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 第一格
                    sim.Mouse.MoveMouseTo(300 * xf, 520 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 服役
                    sim.Mouse.MoveMouseTo(370 * xf, 470 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // [確定]
                    sim.Mouse.MoveMouseTo(525 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 第三種船
                    sim.Mouse.MoveMouseTo(200 * xf, (260 + (3 * 34) - 34) * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 第二格
                    sim.Mouse.MoveMouseTo(385 * xf, 520 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 服役
                    sim.Mouse.MoveMouseTo(370 * xf, 470 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // [確定]
                    sim.Mouse.MoveMouseTo(525 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 第四種船
                    sim.Mouse.MoveMouseTo(200 * xf, (260 + (4 * 34) - 34) * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 第三格
                    sim.Mouse.MoveMouseTo(480 * xf, 520 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 服役
                    sim.Mouse.MoveMouseTo(370 * xf, 470 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // [確定]
                    sim.Mouse.MoveMouseTo(525 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 艦隊名
                    sim.Mouse.MoveMouseTo(380 * xf, 210 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    sim.Keyboard.TextEntry("a").Sleep(100);
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
                    #endregion
                } else if (Stage == 4) {
                    // 研究
                    sim.Mouse.MoveMouseTo(850 * xf, 654 * yf).Sleep(100).LeftButtonClick().Sleep(1500);
                    // 第一次確定
                    sim.Mouse.MoveMouseTo(608 * xf, 493 * yf).Sleep(100).LeftButtonClick().Sleep(600);
                    // 工業
                    sim.Mouse.MoveMouseTo(200 * xf, 243 * yf).Sleep(100).LeftButtonClick().Sleep(400);
                    sim.Mouse.MoveMouseTo(515 * xf, 460 * yf).Sleep(100).LeftButtonClick().Sleep(1200);
                    // 殖民
                    sim.Mouse.MoveMouseTo(200 * xf, 268 * yf).Sleep(100).LeftButtonClick().Sleep(400);
                    sim.Mouse.MoveMouseTo(515 * xf, 460 * yf).Sleep(100).LeftButtonClick().Sleep(1200);
                    // 娛樂科技
                    sim.Mouse.MoveMouseTo(200 * xf, 288 * yf).Sleep(100).LeftButtonClick().Sleep(400);
                    sim.Mouse.MoveMouseTo(515 * xf, 460 * yf).Sleep(100).LeftButtonClick().Sleep(1200);
                    // close
                    sim.Mouse.MoveMouseTo(615 * xf, 635 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                } else if (Stage == 5) {
                    // 殖民
                    sim.Mouse.MoveMouseTo(987 * xf, 406 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // M874III
                    sim.Mouse.MoveMouseTo(262 * xf, 705 * yf).Sleep(100).LeftButtonClick().Sleep(2000);
                    // 確定
                    sim.Mouse.MoveMouseTo(600 * xf, 490 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 點船
                    sim.Mouse.MoveMouseTo(356 * xf, 438 * yf).Sleep(100).LeftButtonClick().Sleep(1000);
                    // 解散
                    sim.Mouse.MoveMouseTo(911 * xf, 485 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // 確定
                    sim.Mouse.MoveMouseTo(500 * xf, 460 * yf).Sleep(100).LeftButtonClick().Sleep(300);
                    // M874II
                    sim.Mouse.MoveMouseTo(307 * xf, 443 * yf).Sleep(100).LeftButtonClick().Sleep(2000);
                    // 託管
                    sim.Mouse.MoveMouseTo(350 * xf, 450 * yf).Sleep(100).LeftButtonClick().Sleep(2000);
                }
            }
        }

        public override string ToString () {
            return "第2階段特殊指令: stage" + Stage;
        }

    }

}
