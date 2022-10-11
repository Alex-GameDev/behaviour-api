using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    /// <summary>
    /// Reprsents a link between two nodes.
    /// </summary>
    public abstract class Connection : ScriptableObject
    {
        /// <summary>
        /// The graph that contains this connection.
        /// </summary>
        public BehaviourEngine Graph { get; private set; }

        /// <summary>
        /// The Node where this connection starts
        /// </summary>
        public Node SourceNode { get; set; }

        /// <summary>
        /// The node where this connection ends
        /// </summary>
        public Node TargetNode { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Connection()
        {
        }
    }
}