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
        public ExecutionStatusPerception(IStatusHandler statusHandler, StatusFlags flags = StatusFlags.Running)
        {
            StatusHandler = statusHandler;
            StatusFlags = flags;
        }

        public override bool Check()
        {
            StatusFlags handlerStatusFlag = (StatusFlags) StatusHandler.Status;
            return (handlerStatusFlag & StatusFlags) != 0;
        }
    }

    [System.Flags]
    public enum StatusFlags
    {
        /// <summary>
        /// Equivalent to Status.None
        /// </summary>
        None = 0,

        /// <summary>
        /// Equivalent to Status.Running
        /// </summary>
        Running = 1,

        /// <summary>
        /// Equivalent to Status.Success
        /// </summary>
        Success = 2,

        /// <summary>
        /// Equivalent to Status.Running | Status.Success
        /// </summary>
        NotFailure = 3,

        /// <summary>
        /// Equivalent to Status.Failure
        /// </summary>
        Failure = 4,

        /// <summary>
        /// Equivalent to Status.Running | Status.Failure
        /// </summary>
        NotSuccess = 5,

        /// <summary>
        /// Equivalent to Status.Success | Status.Failure
        /// </summary>
        Finished = 6,

        /// <summary>
        /// Equivalent to Status.Running | Status.Success | Status.Failure
        /// </summary>
        Actived = 7
    }
}
