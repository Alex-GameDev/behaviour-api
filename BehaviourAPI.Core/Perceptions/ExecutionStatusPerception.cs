namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Perception used to check the IStatusHandler's execution status.
    /// </summary>
    public class ExecutionStatusPerception : Perception
    {
        public IStatusHandler StatusHandler;

        public StatusFlags StatusFlags;

        /// <summary>
        /// Create a perception that checks if a element's status matches the given flags.
        /// </summary>
        /// <param name="statusHandler">The element checked</param>
        public ExecutionStatusPerception(IStatusHandler statusHandler, StatusFlags flags)
        {
            StatusHandler = statusHandler;
            StatusFlags = flags;
        }

        /// <summary>
        /// Create a perception that checks if a element finished its execution with success or failure.
        /// </summary>
        /// <param name="statusHandler">The element checked</param>
        /// <param name="onSuccess">Set to true if perception must trigger when status is Success</param>
        /// <param name="OnFailure">Set to true if perception must trigger when status is Failure</param>
        public ExecutionStatusPerception(IStatusHandler statusHandler, bool onSuccess, bool OnFailure)
        {
            StatusHandler = statusHandler;
            if (onSuccess) StatusFlags |= StatusFlags.Success;
            if (OnFailure) StatusFlags |= StatusFlags.Failure;
        }

        /// <summary>
        /// Create a perception that checks if a element is running
        /// </summary>
        /// <param name="statusHandler">The element checked</param>
        public ExecutionStatusPerception(IStatusHandler statusHandler)
        {
            StatusHandler = statusHandler;
            StatusFlags |= StatusFlags.Running;
        }

        public override bool Check()
        {
            StatusFlags handlerStatusFlag = StatusHandler.Status.GetFlags();
            return (handlerStatusFlag & StatusFlags) != 0;
        }
    }

    [System.Flags]
    public enum StatusFlags
    {
        None = 0,
        Running = 1,
        Success = 2,
        Failure = 4
    }
}
