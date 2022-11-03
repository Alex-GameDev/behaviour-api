using System;
using System.Collections;
using System.Collections.Generic;

namespace BehaviourAPI.Core
{
    /// <summary>
    /// Represents a link between two nodes.
    /// </summary>
    public abstract class Connection : GraphElement
    {
        /// <summary>
        /// The Node where this connection starts
        /// </summary>
        public Node? SourceNode;

        /// <summary>
        /// The node where this connection ends
        /// </summary>
        public Node? TargetNode;

    }
}