using UnityEngine;
using System.Collections.Generic;
using System;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class BehaviourEngine : ScriptableObject, IStatusHandler
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
        public List<Node> Nodes = new List<Node>();

        /// <summary>
        /// The list of all <see cref="Connection"/> elements in ths <see cref="BehaviourEngine"/>
        /// </summary>
        public List<Connection> Connections = new List<Connection>();

        /// <summary>
        /// The default entry point of the graph
        /// </summary>
        public Node StartNode
        {
            get => m_startNode;
            set { if (m_startNode != value) { StartNodeChanged?.Invoke(m_startNode, value); m_startNode = value; } }
        }

        /// <summary>
        /// The current executed node.
        /// </summary>
        public Node CurrentNode
        {
            get => m_currentNode;
            set { if (m_currentNode != value) { CurrentNodeChanged?.Invoke(m_currentNode, value); m_currentNode = value; } }
        }

        /// <summary>
        /// The current execution status of the graph.
        /// </summary>
        public Status Status { get; protected set; }

        public Action<Node, Node> StartNodeChanged;
        public Action<Node, Node> CurrentNodeChanged;

        [SerializeField] Node m_currentNode;
        Node m_startNode;

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
            if (StartNode == null)
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
            if (StartNode == node) StartNode = null;
        }

        public virtual void RecalculateStartNode()
        {
            if (StartNode != null) return;
            StartNode = Nodes.Find(node => node.InputConnections.Count == 0);
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
        public virtual void Initialize(Context context)
        {
            Nodes.ForEach((node) => node.Initialize(context));
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
