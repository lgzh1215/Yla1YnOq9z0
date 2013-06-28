using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SSWSyncer.Commands {

    [Serializable]
    class OpenFacilityBuildPanelCommand : AbstractCommand {

        static readonly private Dictionary<FacilityType, string> facility = new Dictionary<FacilityType, string> {
            {FacilityType.Tita, "鈦礦"},
            {FacilityType.He3, "氦3井"},
            {FacilityType.Loli, "居住區"},
            {FacilityType.Research, "科研區"},
            {FacilityType.Dock, "造船區"},
            {FacilityType.Defense, "軍事區"},
        };

        static readonly private Dictionary<string, FacilityType> 設施 = new Dictionary<string, FacilityType> {
            {"鈦礦", FacilityType.Tita},
            {"氦3井", FacilityType.He3},
            {"居住區", FacilityType.Loli},
            {"科研區", FacilityType.Research},
            {"造船區", FacilityType.Dock},
            {"軍事區", FacilityType.Defense},
        };

        static public Dictionary<FacilityType, string> Facility {
            get {
                return facility;
            }
        }

        public FacilityType Type { get; set; }

        public OpenFacilityBuildPanelCommand (FacilityType type) {
            Type = type;
        }

        public OpenFacilityBuildPanelCommand (Dictionary<string, object> context) {
            ComboBox Combobox = context["Facility"] as ComboBox;
            string selected = Combobox.SelectedItem as string;
            Type = 設施[selected];
        }

        public override void Invoke (bool isSimulate) {
            log.Debug("Open facility build panel");
            StateContainer.EnterResourceBuildingList();
            if (isSimulate) {
                Point target;
                switch (Type) {
                    case FacilityType.Tita:
                        target = new Point(118, 644);
                        break;
                    case FacilityType.He3:
                        target = new Point(784, 654);
                        break;
                    case FacilityType.Loli:
                        target = new Point(308, 460);
                        break;
                    case FacilityType.Research:
                        target = new Point(463, 600);
                        break;
                    case FacilityType.Dock:
                        target = new Point(666, 156);
                        break;
                    case FacilityType.Defense:
                        target = new Point(600, 475);
                        break;
                    default:
                        target = new Point(118, 644);
                        break;
                }
                sim.Mouse.MoveMouseTo(target.X * xf, target.Y * yf).Sleep(100).LeftButtonClick().Sleep(1000);
            }
        }

        public override string ToString () {
            return "打開設施建造列表(" + Facility[Type] + ")";
        }

    }

}
