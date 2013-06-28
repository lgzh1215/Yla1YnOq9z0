using System;

namespace SSWSyncer.Commands {

    class NotSupportedCommandException : NotSupportedException {

        public int Index { get; private set; }

        public Command Command { get; private set; }

        public NotSupportedCommandException (int index, Command command)
            : base() {
            Index = index;
            Command = command;
        }

    }

}
