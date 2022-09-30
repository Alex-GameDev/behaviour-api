using System;
using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using Core;
    /// <summary>
    /// Node that inverts the result returned by its child node (Success/Failure).
    /// </summary>

    public class InverterNode : DecoratorNode
    {
        protected override Status GetModifiedChildStatus(Status childStatus)
        {
            if (childStatus == Status.Sucess) return Status.Failure;
            if (childStatus == Status.Failure) return Status.Sucess;
            else return childStatus;
        }
    }
}