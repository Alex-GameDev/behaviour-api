namespace BehaviourAPI.StateMachines
{
    using BehaviourAPI.Core.Actions;
    using Core;

    /// <summary>
    /// A State node that executes an Action.
    /// </summary>
    public class ActionState : State, IActionHandler
    {

        #region ------------------------------------------ Properties -----------------------------------------

        public Action? Action { get => _action; set => _action = value; }

        Action? _action;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public ActionState SetAction(Action action)
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
