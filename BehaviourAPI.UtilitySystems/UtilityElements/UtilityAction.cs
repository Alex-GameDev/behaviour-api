namespace BehaviourAPI.UtilitySystems
{
    using BehaviourAPI.Core.Actions;
    using Core;
    /// <summary>
    /// Utility element that handle multiple <see cref="UtilitySelectableNode"/> itself and
    /// returns the maximum utility if its best candidate utility is higher than the threshold.
    /// </summary>
    public class UtilityAction : UtilityElement, IActionHandler
    {
        #region ------------------------------------------ Properties ----------------------------------------

        public override string Description => "Utility element that executes an action.";
        public Action? Action { get; set; }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            Action?.Start();
        }

        protected override Status UpdateStatus()
        {
            Status = Action?.Update() ?? Status.Error;
            return Status;
        }

        public override void Stop()
        {
            Action?.Stop();
        }
        #endregion
    }
}
