using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class Node : ScriptableObject
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
        /// The type of the nodes that this node can handle as a childs.
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

        #region Event
        public Action<int> InputConnectionAdded;
        public Action<int> OutputConnectionAdded;
        public Action<int> InputConnectionRemoved;
        public Action<int> OutputConnectionRemoved;
        #endregion

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

        public bool IsConnectedWith(Node node)
        {
            return IsParentOf(node) || IsChildOf(node);
        }

        /// <summary>
        /// Called when another node is connected as a child of this.
        /// </summary>
        /// <param name="connection">The added connection.</param>
        /// <param name="index">The connection index.</param>
        public virtual void OnChildNodeConnected(Connection connection, int index)
        {
            OutputConnections.Insert(index, connection);
            OutputConnectionAdded?.Invoke(index);
        }

        /// <summary>
        /// Called when another node is connected as a parent of this.
        /// </summary>
        /// <param name="connection">The added connection.</param>
        /// <param name="index">The connection index.</param>
        public virtual void OnParentNodeConnected(Connection connection, int index)
        {
            InputConnections.Insert(index, connection);
            InputConnectionAdded?.Invoke(index);
        }

        /// <summary>
        /// Called when a child node is disconnected from this.
        /// </summary>
        /// <param name="connection">The removed connection.</param>
        public virtual void OnChildNodeDisconnected(Connection connection)
        {
            int idx = OutputConnections.IndexOf(connection);
            OutputConnections.Remove(connection);
            OutputConnectionRemoved?.Invoke(idx);
        }

        /// <summary>
        /// Called when a parent node is disconnected from this.
        /// </summary>
        /// <param name="connection">The removed connection.</param>
        public virtual void OnParentNodeDisconnected(Connection connection)
        {
            int idx = InputConnections.IndexOf(connection);
            InputConnections.Remove(connection);
            InputConnectionRemoved?.Invoke(idx);
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
