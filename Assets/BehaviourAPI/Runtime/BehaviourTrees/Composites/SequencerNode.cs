using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    /// <summary>
    /// Composite node that executes its children until one of them returns Failure.
    /// </summary>
    public class SequencerNode : CompositeNode
    {
        public SequencerNode()
        {
        }
    }
}