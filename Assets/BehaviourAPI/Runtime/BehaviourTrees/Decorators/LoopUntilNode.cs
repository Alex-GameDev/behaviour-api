using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using Core;
    /// <summary>
    /// Node that execute its child node until returns a given value.
    /// </summary>
    public class LoopUntilNode : DecoratorNode
    {
        public Status TargetStatus { get; set; }
    }
}