using UnityEngine;
using System.Collections.Generic;
using System;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class BehaviourEngine
    {

        /// <summary>
        /// The base type of the <see cref="Node"/> elements that this <see cref="BehaviourEngine"/> can contain.
        /// </summary>
        public abstract System.Type NodeType { get; }

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
                Node node = (Node)System.Activator.CreateInstance(type);
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
        public Connection CreateConnection(System.Type type, Node source, Node target,
            int sourceIndex = -1, int targetIndex = -1)
        {
            if (type.IsSubclassOf(NodeType) && Nodes.Contains(source) && Nodes.Contains(target))
            {
                Connection connection = (Connection)System.Activator.CreateInstance(type);
                connection.SourceNode = source;
                connection.TargetNode = target;
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
        protected virtual bool AddNode(Node node)
        {
            if (Nodes.Count == 0)
                StartNode = node;

            Nodes.Add(node);
            return true;
        }

        /// <summary>
        /// Add a connection to the graph
        /// </summary>
        /// <param name="node"></param>
        protected virtual bool AddConnection(Connection connection)
        {
            Connections.Add(connection);
            return true;
        }

        /// <summary>
        /// Remove a specific node from the graph.
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(Node node)
        {
            Nodes.Remove(node);
        }

        /// <summary>
        /// Remove an specific connection from the graph.
        /// </summary>
        /// <param name="connection"></param>
        public void RemoveConnection(Connection connection)
        {
            connection.Disconnect();
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

        /// <summary>
        /// Enter this behavior graph from a subgraph node or a <see cref="BehaviourRunner"/>
        /// </summary>
        public virtual void Entry()
        {
            EntryNode(StartNode);
        }

        /// <summary>
        /// Call every execution frame.
        /// </summary>
        public virtual void Update()
        {
            CurrentNode.Update();
        }

        /// <summary>
        /// Entry in a node.
        /// </summary>
        /// <param name="node">The new Current Node</param>
        public virtual void EntryNode(Node node)
        {
            CurrentNode = node;
            CurrentNode.Entry();
        }

        #endregion
    }
}
