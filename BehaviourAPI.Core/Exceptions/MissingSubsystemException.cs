using BehaviourAPI.Core.Actions;
using System;
using System.Runtime.Serialization;

namespace BehaviourAPI.Core.Exceptions
{
    public class MissingSubsystemException : NullReferenceException
    {
        public EnterSystemAction SubsystemAction;

        public MissingSubsystemException(EnterSystemAction subsystemAction)
        {
            SubsystemAction = subsystemAction;
        }

        public MissingSubsystemException(EnterSystemAction subsystemAction, string message) : base(message)
        {
            SubsystemAction = subsystemAction;
        }

        public MissingSubsystemException(EnterSystemAction subsystemAction, string message, Exception innerException) : base(message, innerException)
        {
            SubsystemAction = subsystemAction;
        }

        protected MissingSubsystemException(EnterSystemAction subsystemAction, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            SubsystemAction = subsystemAction;
        }
    }
}
