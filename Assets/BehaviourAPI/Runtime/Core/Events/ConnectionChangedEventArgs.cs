using System;

namespace BehaviourAPI.Runtime.Core
{
    public class ConnectionChangedEventArgs : EventArgs
    {
        public ConnectionEventType EventType;
        public ConnectionDirection Direction;
        public int Index;

        public ConnectionChangedEventArgs(ConnectionEventType type, ConnectionDirection dir, int index)
        {
            EventType = type;
            Direction = dir;
            Index = index;
        }
    }

    public enum ConnectionEventType
    {
        ADD = 0,
        REMOVE = 1
    }

    public enum ConnectionDirection
    {
        INPUT = 0,
        OUTPUT = 1
    }
}
