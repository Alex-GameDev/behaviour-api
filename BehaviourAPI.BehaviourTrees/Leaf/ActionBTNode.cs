namespace BehaviourAPI.BehaviourTrees
{
    using BehaviourAPI.Core.Actions;
    using Core;

    /// <summary>
    /// A behaviour tree node that executes an Action.
    /// </summary>
    public class ActionBTNode : LeafNode, IActionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override string Description => "Behaviour Tree Node that executes an Action";
        public Action? Action { get; set; }

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public ActionBTNode SetAction(Action action)
        {
            Action = action;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            Action?.Start();
        }

        protected override Status UpdateStatus()
        {
            Status = Action?.Update() ?? Status.Error;
            return Status;
        }

        public override void Stop()
        {
            base.Stop();
            Action?.Stop();
        }

        #endregion
    }
}