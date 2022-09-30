using System.Collections.Generic;
using BehaviourAPI.Runtime.Core;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    /// <summary>
    /// Node that changes the result returned by its child node to Succeded if it's Failure.
    /// </summary>
    public class SuccederNode : DecoratorNode
    {

        protected override Status GetModifiedChildStatus(Status childStatus)
        {
            return childStatus == Status.Failure ? Status.Sucess : childStatus;
        }
    }
}