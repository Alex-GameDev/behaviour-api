using BehaviourAPI.Core.Actions;
using System;
using System.Runtime.Serialization;

namespace BehaviourAPI.Core.Exceptions
{
    public class MissingSubsystemException : NullReferenceException
    {
        public SubsystemAction SubsystemAction;

        public MissingSubsystemException(SubsystemAction subsystemAction)
        {
            SubsystemAction = subsystemAction;
        }

        public MissingSubsystemException(SubsystemAction subsystemAction, string message) : base(message)
        {
            SubsystemAction = subsystemAction;
        }

        public MissingSubsystemException(SubsystemAction subsystemAction, string message, Exception innerException) : base(message, innerException)
        {
            SubsystemAction = subsystemAction;
        }

        protected MissingSubsystemException(SubsystemAction subsystemAction, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            SubsystemAction = subsystemAction;
        }
    }
}
