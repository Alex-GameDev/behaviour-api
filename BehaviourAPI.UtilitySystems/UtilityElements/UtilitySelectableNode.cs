namespace BehaviourAPI.UtilitySystems
{
    using Core;

    /// <summary>
    /// Utility node that implements IStatusHandler
    /// </summary>
    public abstract class UtilitySelectableNode : UtilityNode, IStatusHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Status Status { get => _status; protected set => _status = value; }
        Status _status;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public abstract void Start();

        public abstract void Update();

        public abstract void Stop();

        #endregion
    }
}