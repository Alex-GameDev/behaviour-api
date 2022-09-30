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
        public Status TargetStatus { get; set; }

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