namespace BehaviourAPI.BehaviourTrees
{
    using Core;

    /// <summary>
    /// A behaviour tree node that executes an Action.
    /// </summary>
    public class ActionBTNode : LeafNode, IActionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override string Description => "Behaviour Tree Node that executes an Action";
        public Func<ExecutionPhase, Status>? Action { get; set; }

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public ActionBTNode SetAction(Func<ExecutionPhase, Status> action)
        {
            Action = action;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            Action?.Invoke(ExecutionPhase.START);
        }

        protected override Status UpdateStatus()
        {
            Status = Action?.Invoke(ExecutionPhase.UPDATE) ?? Status.Error;
            return Status;
        }

        public override void Stop()
        {
            base.Stop();
            Action?.Invoke(ExecutionPhase.STOP);
        }

        #endregion
    }
}