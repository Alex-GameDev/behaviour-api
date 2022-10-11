using UnityEngine;
using System.Collections.Generic;
using System;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class BehaviourEngine : IStatusHandler
    {

        /// <summary>
        /// The base type of the <see cref="Node"/> elements that this <see cref="BehaviourEngine"/> can contain.
        /// </summary>
        public abstract System.Type NodeType { get; }

        /// <summary>
        /// The base type of the <see cref="Connection"/> elements that this <see cref="BehaviourEngine"/> can contain.
        /// </summary>
        public abstract System.Type ConnectionType { get; }

        /// <summary>
        /// The list of all <see cref="Node"/> elements in this <see cref="BehaviourEngine"/>.
        /// </summary>
        public List<Node> Nodes { get; }

        /// <summary>
        /// The list of all <see cref="Connection"/> elements in ths <see cref="BehaviourEngine"/>
        /// </summary>
        public List<Connection> Connections { get; }

        /// <summary>
        /// The default entry point of the graph
        /// </summary>
        public Node StartNode { get; set; }

        /// <summary>
        /// The current executed node.
        /// </summary>
        public Node CurrentNode { get; private set; }

        /// <summary>
        /// The current execution status of the graph.
        /// </summary>
        public Status Status { get; protected set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public BehaviourEngine()
        {
            Nodes = new List<Node>();
            Connections = new List<Connection>();
        }

        #region Create and remove elements
        /// <summary>
        /// Create a new node in the graph. Nodes should only be created with this method.
        /// </summary>
        public Node CreateNode(System.Type type, Vector2 pos = default)
        {
            if (type.IsSubclassOf(NodeType))
            {
                Node node = (Node)ScriptableObject.CreateInstance(type);
                node.Position = pos;
                node.BehaviourGraph = this;

                AddNode(node);

                return node;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create a new Connection in the graph. Connections should only be created with this method.
        /// </summary>
        public Connection CreateConnection(Node source, Node target,
            int sourceIndex = -1, int targetIndex = -1)
        {
            if (Nodes.Contains(source) && Nodes.Contains(target))
            {
                Connection connection = (Connection)ScriptableObject.CreateInstance(ConnectionType);
                connection.SourceNode = source;
                connection.TargetNode = target;
                AddConnection(connection, sourceIndex, targetIndex);
                return connection;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Add a node to the graph
        /// </summary>
        /// <param name="node"></param>
        protected virtual void AddNode(Node node)
        {
            if (Nodes.Count == 0)
                StartNode = node;

            Nodes.Add(node);
        }

        /// <summary>
        /// Add a connection to the graph
        /// </summary>
        /// <param name="node"></param>
        protected virtual void AddConnection(Connection connection, int sourceIndex, int TargetIndex)
        {
            Connections.Add(connection);
            connection.SourceNode.OnChildNodeConnected(connection, sourceIndex);
            connection.TargetNode.OnParentNodeConnected(connection, TargetIndex);
        }

        /// <summary>
        /// Remove a specific node from the graph.
        /// </summary>
        /// <param name="node"></param>
        public virtual void RemoveNode(Node node)
        {
            Nodes.Remove(node);
        }

        /// <summary>
        /// Remove an specific connection from the graph.
        /// </summary>
        /// <param name="connection"></param>
        public virtual void RemoveConnection(Connection connection)
        {
            connection.SourceNode.OnChildNodeDisconnected(connection);
            connection.TargetNode.OnParentNodeDisconnected(connection);
            Connections.Remove(connection);

        }

        #endregion

        #region Execution Methods

        /// <summary>
        /// Initialize all nodes.
        /// </summary>
        public virtual void Initialize()
        {
            Nodes.ForEach((node) => node.Initialize());
        }

        public virtual void SetCurrentNode(Node node)
        {
            CurrentNode = node;
        }

        /// <summary>
        /// Enter this behavior graph from a subgraph node or a <see cref="BehaviourRunner"/>
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Executes every frame and 
        /// </summary>
        /// <returns></returns>

        public abstract void Update();

        #endregion
    }
}
