using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;

namespace BehaviourAPI.BehaviourTrees
{
    using Core.Actions;
    /// <summary>
    /// BTNode type that has no children.
    /// </summary>
    public class LeafNode : BTNode, IActionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public sealed override int MaxOutputConnections => 0;
        public Action? Action { get; set; }     

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public LeafNode SetAction(Action action)
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
