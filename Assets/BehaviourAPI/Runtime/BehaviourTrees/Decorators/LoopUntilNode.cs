using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using System;
    using Core;
    /// <summary>
    /// Node that execute its child node until returns a given value.
    /// </summary>
    public class LoopUntilNode : DecoratorNode
    {
        public override string Name => "Loop Until";
        public override string Description => "Decorator node that executes its child until it returns the desired value";
        public Status TargetStatus;

        protected override Status GetModifiedChildStatus(Status childStatus)
        {
            if (childStatus != TargetStatus && (childStatus == Status.Failure || childStatus == Status.Sucess))
            {
                ResetChild();
                return Status.Running;
            }
            else
            {
                return childStatus;
            }
        }
    }
}