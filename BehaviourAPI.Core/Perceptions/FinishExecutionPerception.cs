namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Perception used to check the IStatusHandler's finish execution status.
    /// </summary>
    public class FinishExecutionPerception : Perception
    {
        public IStatusHandler StatusHandler;

        public bool TriggerOnSuccess;
        public bool TriggerOnFailure;

        public FinishExecutionPerception(IStatusHandler statusHandler, bool triggerOnSuccess, bool triggerOnFailure)
        {
            StatusHandler = statusHandler;
            TriggerOnSuccess = triggerOnSuccess;
            TriggerOnFailure = triggerOnFailure;
        }

        public override bool Check()
        {
            return ((StatusHandler.Status == Status.Success && TriggerOnSuccess) ||
                (StatusHandler.Status == Status.Failure && TriggerOnFailure));
        }
    }
}
