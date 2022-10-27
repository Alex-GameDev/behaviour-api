namespace BehaviourAPI.BehaviourTrees
{
    using Core;

    /// <summary>
    /// The base node in the <see cref="BehaviourTree"/>.
    /// </summary>
    public abstract class BTNode : Node, IStatusHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override int MaxInputConnections => 1;
        public override Type ChildType => typeof(BTNode);   

        public Status Status { get; set; }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public virtual void Start()
        {
            Status = Status.Running;
        }

        public void Update()
        {
            Status = UpdateStatus();
        }

        public virtual void Stop()
        {
            Status = Status.None;
        }

        protected abstract Status UpdateStatus();

        #endregion
    }
}