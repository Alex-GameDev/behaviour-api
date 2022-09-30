using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class Node
    {
        /// <summary>
        /// Node Name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Node description.
        /// </summary>
        public virtual string Desc { get; set; }

        /// <summary>
        /// The graph that contains this node.
        /// </summary>
        public BehaviourEngine BehaviourGraph { get; set; }

        /// <summary>
        /// The Node visual position (Editor)
        /// </summary>
        public Vector2 Position { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public virtual Type ChildType { get; } = typeof(Node);

        /// <summary>
        /// Maximum number of <see cref="Connection"/> elements in <see cref="InputConnections"/>.
        /// </summary>
        public abstract int MaxInputConnections { get; }

        /// <summary>
        /// Maximum number of <see cref="Connection"/> elements in <see cref="OutputConnections"/>.
        /// </summary>
        public abstract int MaxOutputConnections { get; }

        /// <summary>
        /// List of connections with this node as target.
        /// </summary>
        public List<Connection> InputConnections = new List<Connection>();

        /// <summary>
        /// List of connections with this node as source.
        /// </summary>
        public List<Connection> OutputConnections = new List<Connection>();

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Node() { }

        /// <summary>
        /// Get all nodes connected with this as source.
        /// </summary>
        /// <returns>List of parent nodes</returns>
        public Node[] GetParentNodes()
        {
            return InputConnections.Select((x) => x.SourceNode).Distinct().ToArray();
        }

        /// <summary>
        /// Get all nodes connected with this as target.
        /// </summary>
        /// <returns>List of child nodes</returns>
        public Node[] GetChildNodes()
        {
            return OutputConnections.Select((x) => x.TargetNode).Distinct().ToArray();
        }

        /// <summary>
        /// Checks if a node is connected with this as target.
        /// </summary>
        /// <param name="node">The checked target node</param>
        /// <returns>True if the checked node is a child of this node.</returns>
        public bool IsChildOf(Node node)
        {
            return InputConnections.Select((x) => x.SourceNode).Contains(node);
        }

        /// <summary>
        /// Checks if a node is connected with this as source.
        /// </summary>
        /// <param name="node">The checked source node</param>
        /// <returns>True if the checked node is a parent of this node.</returns>
        public bool IsParentOf(Node node)
        {
            return OutputConnections.Select((x) => x.TargetNode).Contains(node);
        }

        public virtual void OnChildNodeConnected(Connection connection, int index)
        {
            OutputConnections.Insert(index, connection);
        }
        public virtual void OnParentNodeConnected(Connection connection, int index)
        {
            InputConnections.Insert(index, connection);
        }
        public virtual void OnChildNodeDisconnected(Connection connection)
        {
            OutputConnections.Remove(connection);
        }
        public virtual void OnParentNodeDisconnected(Connection connection)
        {
            InputConnections.Remove(connection);
        }

        /// <summary>
        /// Initialize the node
        /// </summary>
        public virtual void Initialize()
        {

        }

        public void OnRemoved()
        {
            BehaviourGraph.RemoveNode(this);
        }
    }
}
