namespace BehaviourAPI.BehaviourTrees.Composites
{
    using BehaviourAPI.Core.Exceptions;
    using Core;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Composite node that executes its children at the same time, until one of them returns the trigger value.
    /// </summary>
    public class ParallelCompositeNode : CompositeNode
    {
        #region ------------------------------------------- Fields -------------------------------------------

        public Status TriggerStatus = Status.Failure;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------
        public override void Start()
        {
            base.Start();

            if (m_children.Count == 0)
                throw new MissingChildException(this);

            m_children.ForEach(c => c?.Start());
        }

        public override void Stop()
        {
            base.Stop();

            if (m_children.Count == 0)
                throw new MissingChildException(this);

            m_children.ForEach(c => c?.Stop());
        }

        protected override Status UpdateStatus()
        {
            if (m_children.Count == 0)
                throw new MissingChildException(this);

            m_children.ForEach(c => c.Update());
            List<Status> allStatus = m_children.Select(c => c.Status).ToList();

            // Check for trigger value
            if (allStatus.Contains(TriggerStatus)) return TriggerStatus;

            // Check if execution finish
            if (!allStatus.Contains(Status.Running)) return TriggerStatus == Status.Success ? Status.Failure : Status.Success;

            return Status.Running;
        }
        #endregion
    }
}
