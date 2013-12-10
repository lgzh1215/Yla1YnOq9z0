using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanColleTool {

    class MissionDetail {

        public string Name { get; private set; }

        public int Id { get; private set; }

        public MissionDetail (int id, string name) {
            Id = id;
            Name = name;
        }

        public override string ToString () {
            return Name;
        }
    }
}
