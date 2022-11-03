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

        public Action? Action { get => _action; set => _action = value; }

        Action? _action;

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