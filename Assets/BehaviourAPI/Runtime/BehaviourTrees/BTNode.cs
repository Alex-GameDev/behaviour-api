using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using System;
    using Core;

    /// <summary>
    /// The base node in the <see cref="BehaviourTree"/>.
    /// </summary>
    public abstract class BTNode : Node
    {
        public override int MaxInputConnections => 1;

        public override Type ChildType => typeof(BTNode);

        public BTNode()
        {
        }
    }
}