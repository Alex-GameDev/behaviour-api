namespace BehaviourAPI.UtilitySystems
{
    using Core;

    /// <summary>
    /// Utility node that implements IStatusHandler
    /// </summary>
    public abstract class UtilitySelectableNode : UtilityNode, IStatusHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Status Status { get; protected set; }

        /// <summary>
        /// True if this element should be executed even if later elements have more utility:
        /// </summary>
        public bool ExecutionPriority { get; protected set; }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public abstract void Start();

        public abstract void Update();

        public abstract void Stop();

        /// <summary>
        /// Return true if the utility system should change its status when a selectable node finish its execution
        /// </summary>
        /// <returns></returns>
        public abstract bool FinishExecutionWhenActionFinishes(); 
        #endregion
    }
}