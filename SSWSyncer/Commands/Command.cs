using System.ComponentModel;

namespace SSWSyncer.Commands {

    public interface Command {

        void Invoke (bool isSimulate);

    }

    public enum FacilityType {
        [Description("鈦礦")]
        Tita = 1,
        [Description("氦3井")]
        He3 = 2,
        [Description("居住區")]
        Loli = 3,
        [Description("科研區")]
        Research = 4,
        [Description("造船區")]
        Dock = 5,
        [Description("軍事區")]
        Defense = 6,
    }

}
