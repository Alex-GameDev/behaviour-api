using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using Core;

    /// <summary>
    /// A behaviour tree node that executes an <see cref="ActionTask"/>.
    /// </summary>
    public class ActionBTNode : BTNode
    {
        public sealed override int MaxOutputConnections => 0;
        public ActionTask Action { get; set; }

        public override void Entry()
        {
            base.Entry();
            Action.Start();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override Status Update()
        {
            Action.Update();
            return Action.ExecutionStatus;
        }
    }
}