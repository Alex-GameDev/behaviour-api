namespace BehaviourAPI.BehaviourTrees
{
    using System;
    using System.Linq;
    using Core;
    /// <summary>
    /// Composite node that executes its children sequencially.
    /// </summary>
    public abstract class SerialCompositeNode : CompositeNode
    {
        #region ------------------------------------------ Properties -----------------------------------------

        /// <summary>
        /// The Status that, when is returned by a child, keeps the execution to the next.
        /// </summary>
        public abstract Status KeepExecutingStatus { get; }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        int currentChildIdx = 0;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            currentChildIdx = 0;
            base.Start();
            GetCurrentChild()?.Start();
        }

        public override void Stop()
        {
            base.Stop();
            GetCurrentChild()?.Stop();
        }

        protected override Status UpdateStatus()
        {
            BTNode currentChild = GetCurrentChild();
            currentChild?.Update();
            var status = currentChild?.Status ?? Status.Error;

            if (status == KeepExecutingStatus)
            {
                // If there are childs to execute, keep the status to running
                if (TryGoToNextChild()) status = Status.Running;
            }
            return status;
        }

        private bool TryGoToNextChild()
        {
            if (currentChildIdx < ChildCount - 1)
            {
                GetCurrentChild()?.Stop();
                currentChildIdx++;
                GetCurrentChild()?.Start();
                return true;
            }
            else
            {
                return false;
            }
        }

        private BTNode GetCurrentChild() => GetChildAt(currentChildIdx);

        #endregion
    }
}