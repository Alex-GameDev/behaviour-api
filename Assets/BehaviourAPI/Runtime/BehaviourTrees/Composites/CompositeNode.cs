using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    /// <summary>
    /// BTNode type that has multiple children and executes them according to certain conditions.
    /// </summary>
    public abstract class CompositeNode : BTNode
    {
        public sealed override int MaxOutputConnections => -1;
        protected int CurrentChildIndex;
        public BTNode CurrentChild => GetChildNodes()[CurrentChildIndex] as BTNode;

        protected virtual bool TryGoToNextChild()
        {
            CurrentChildIndex += 1;
            return CurrentChildIndex < GetChildNodes().Length;
        }

    }
}