using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class Connection
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

        /// <summary>
        /// Executes the current connection
        /// </summary>
        /// <returns></returns>
        public virtual Status Execute(/*Context ctx*/)
        {
            return SourceNode.Status;
        }

        /// <summary>
        /// Disconnect the connection
        /// </summary>
        public void Disconnect()
        {
            SourceNode.OutputConnections.Remove(this);
            TargetNode.InputConnections.Remove(this);
            SourceNode = null;
            TargetNode = null;
        }
    }
}