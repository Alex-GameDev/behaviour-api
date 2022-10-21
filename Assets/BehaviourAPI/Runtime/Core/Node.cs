using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class Node : GraphElement
    {
        /// <summary>
        /// The node unique name.
        /// </summary>
        [HideInInspector] public string NodeName;
        /// <summary>
        /// The Node visual position (Editor)
        /// </summary>
        [HideInInspector] public Vector2 Position;
        /// <summary>
        /// List of connections with this node as target.
        /// </summary>
        [HideInInspector] public List<Connection> InputConnections;

        /// <summary>
        /// List of connections with this node as source.
        /// </summary>
        [HideInInspector] public List<Connection> OutputConnections;

        /// <summary>
        /// The type of the nodes that this node can handle as a childs.
        /// </summary>
        public virtual Type ChildType => typeof(Node);

        /// <summary>
        /// Maximum number of <see cref="Connection"/> elements in <see cref="InputConnections"/>.
        /// </summary>
        public abstract int MaxInputConnections { get; }

        /// <summary>
        /// Maximum number of <see cref="Connection"/> elements in <see cref="OutputConnections"/>.
        /// </summary>
        public abstract int MaxOutputConnections { get; }

        /// <summary>
        /// Return true if this node is the start node of the graph.
        /// </summary>
        public bool IsStartNode => BehaviourGraph.StartNode == this;

        #region Events
        public event EventHandler<ConnectionChangedEventArgs> ConnectionChanged;

        #endregion

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Node()
        {
            NodeName = $"New {Name}";
            OutputConnections = new List<Connection>();
            InputConnections = new List<Connection>();
        }

        /// <summary>
        /// Get all nodes connected with this as source.
        /// </summary>
        /// <returns>List of parent nodes</returns>
        public IEnumerable<Node> GetParentNodes()
        {
            return InputConnections.Select((x) => x.SourceNode).Distinct();
        }

        /// <summary>
        /// Get all nodes connected with this as target.
        /// </summary>
        /// <returns>List of child nodes</returns>
        public IEnumerable<Node> GetChildNodes()
        {
            return OutputConnections.Select((x) => x.TargetNode).Distinct();
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

        /// <summary>
        /// Check if this node is connected with other node
        /// </summary>
        /// <param name="node">The other node</param>
        /// <returns></returns>
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
            ConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(
                ConnectionEventType.ADD, ConnectionDirection.OUTPUT, index));

        }

        /// <summary>
        /// Called when another node is connected as a parent of this.
        /// </summary>
        /// <param name="connection">The added connection.</param>
        /// <param name="index">The connection index.</param>
        public virtual void OnParentNodeConnected(Connection connection, int index)
        {
            InputConnections.Insert(index, connection);
            ConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(
                ConnectionEventType.ADD, ConnectionDirection.INPUT, index));
        }

        /// <summary>
        /// Called when a child node is disconnected from this.
        /// </summary>
        /// <param name="connection">The removed connection.</param>
        public virtual void OnChildNodeDisconnected(Connection connection)
        {
            int idx = OutputConnections.IndexOf(connection);
            OutputConnections.Remove(connection);
            ConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(
                ConnectionEventType.REMOVE, ConnectionDirection.OUTPUT, idx));
        }

        /// <summary>
        /// Called when a parent node is disconnected from this.
        /// </summary>
        /// <param name="connection">The removed connection.</param>
        public virtual void OnParentNodeDisconnected(Connection connection)
        {
            int idx = InputConnections.IndexOf(connection);
            InputConnections.Remove(connection);
            ConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(
                ConnectionEventType.REMOVE, ConnectionDirection.INPUT, idx));
        }

        /// <summary>
        /// Convert this node to the start node of the graph.
        /// </summary>
        public void ConvertToStartNode() => BehaviourGraph.StartNode = this;

        /// <summary>
        /// Initialize the node with the given context.
        /// The context is only needed in the executable nodes, like Action or perception nodes.
        /// </summary>
        public virtual void Initialize(Context context)
        {

        }

        /// <summary>
        /// Reset the node internal values.
        /// </summary>
        public virtual void Reset()
        {

        }
    }
}
