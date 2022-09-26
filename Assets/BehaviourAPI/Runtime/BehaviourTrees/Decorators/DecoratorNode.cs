using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    /// <summary>
    /// BTNode that alters the result returned by its child node or its execution.
    /// </summary>
    public abstract class DecoratorNode : BTNode
    {
        public sealed override int MaxOutputConnections => 1;

    }
}