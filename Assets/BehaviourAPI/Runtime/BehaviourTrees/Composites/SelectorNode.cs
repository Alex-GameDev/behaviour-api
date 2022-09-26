using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    /// <summary>
    /// Composite node that executes its children until one of them returns Succeded.
    /// </summary>
    public class SelectorNode : CompositeNode
    {
        public bool IsRandomOrder { get; set; }
        public SelectorNode()
        {
        }
    }

}