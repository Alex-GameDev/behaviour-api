using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;

namespace BehaviourAPI.BehaviourTrees
{
    using BehaviourAPI.Core.Exceptions;
    using Core.Actions;
    /// <summary>
    /// BTNode type that has no children.
    /// </summary>
    public class LeafNode : BTNode, IActionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override string Description => "BT Node that executes an action";
        public sealed override int MaxOutputConnections => 0;
        public Action Action { get; set; }     

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
            if (Action == null)
                throw new MissingActionException(this, "Leaf nodes need an action to work.");

            Action.Start();
        }

        protected override Status UpdateStatus()
        {
            if (Action == null)
                throw new MissingActionException(this, "Leaf nodes need an action to work.");

            Status = Action.Update();
            return Status;
        }

        public override void Stop()
        {
            base.Stop();

            if (Action == null)
                throw new MissingActionException(this, "Leaf nodes need an action to work.");

            Action.Stop();
        }

        #endregion
    }
}
