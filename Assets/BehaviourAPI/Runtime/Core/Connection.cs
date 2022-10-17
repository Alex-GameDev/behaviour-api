using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    /// <summary>
    /// Reprsents a link between two nodes.
    /// </summary>
    public abstract class Connection : GraphElement
    {
        public override string Name => "Connection";
        public override string Description => "-";

        /// <summary>
        /// The Node where this connection starts
        /// </summary>
        [HideInInspector] public Node SourceNode;

        /// <summary>
        /// The node where this connection ends
        /// </summary>
        [HideInInspector] public Node TargetNode;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Connection()
        {
        }
    }
}