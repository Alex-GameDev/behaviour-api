namespace BehaviourAPI.BehaviourTrees.Composites
{
    using Core;
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
            GetChildren().ForEach(c => c?.Start());
        }
        protected override Status UpdateStatus()
        {
            List<BTNode?> children = GetChildren();
            children.ForEach(c => c?.Update());
            List<Status> allStatus = children.Select(c => c?.Status ?? Status.Error).ToList();

            // Check errors
            if (allStatus.Contains(Status.Error)) return Status.Error;

            // Check for trigger value
            if (allStatus.Contains(TriggerStatus)) return TriggerStatus;

            // Check if execution finish
            if (!allStatus.Contains(Status.Running)) return TriggerStatus == Status.Sucess ? Status.Failure : Status.Sucess;

            return Status.Running;
        }
        #endregion
    }
}
