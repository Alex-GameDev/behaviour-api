using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using Core;

    /// <summary>
    /// The base node in the <see cref="BehaviourTree"/>.
    /// </summary>
    public class BTNode : Node
    {
        public override int MaxInputConnections => 1;
        public override int MaxOutputConnections => 0;

        public BTNode()
        {
        }
    }
}