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
        [HideInInspector] public BehaviourEngine Graph;

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