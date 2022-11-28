namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    using System;
    using System.Net.NetworkInformation;

    /// <summary>
    /// The base node in the <see cref="BehaviourTree"/>.
    /// </summary>
    public abstract class BTNode : Node, IStatusHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override int MaxInputConnections => 1;
        public override Type ChildType => typeof(BTNode);
        public Status Status { get => _status; protected set => _status = value; }
        Status _status;

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