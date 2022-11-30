using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BehaviourAPI.Core
{
    public abstract class BehaviourGraph : BehaviourSystem
    {
        #region ----------------------------------------- Properties -------------------------------------------

        /// <summary>
        /// The base type of the <see cref="Node"/> elements that this <see cref="BehaviourGraph"/> can contain.
        /// </summary>
        public abstract Type NodeType { get; }

        /// <summary>
        /// The default entry point of the graph
        /// </summary>
        public Node StartNode => Nodes[0];

        /// <summary>
        /// True if nodes can have more than one connection with the same node.
        /// </summary>
        public abstract bool CanRepeatConnection { get; }

        /// <summary>
        /// True if connections can create loops.
        /// </summary>
        public abstract bool CanCreateLoops { get; }

        #endregion

        #region ------------------------------------------- Fields ---------------------------------------------

        public List<Node> Nodes = new List<Node>();

        // Used internally to find nodes by name
        Dictionary<string, Node> _nodeDict = new Dictionary<string, Node>();

        // Used internally to reduce complexity of creating connections from O(N) to O(1)
        HashSet<Node> _nodeSet = new HashSet<Node>();

        #endregion

        #region ---------------------------------------- Build methods -----------------------------------------

        /// <summary>
        /// Create a new node of type <typeparamref name="T"/> named <paramref name="name"/> in this Graph.
        /// </summary>
        /// <typeparam name="T">The type of the new node.</typeparam>
        /// <param name="name">The name of the node.</param>
        /// <returns>The created node.</returns>
        protected T CreateNode<T>(string name) where T : Node, new()
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                name = $"Node_{Guid.NewGuid()}";
            }
            T node = new T();
            node.BehaviourGraph = this;
            node.Name = name;
            _nodeDict.Add(name, node);
            Nodes.Add(node);
            _nodeSet.Add(node);
            return node;     
        }

        /// <summary>
        /// Create a new node of type <typeparamref name="T"/> in this Graph.
        /// </summary>
        /// <typeparam name="T">The type of the new node.</typeparam>
        /// <returns>The created node.</returns>
        protected T CreateNode<T>() where T : Node, new() => CreateNode<T>(null);

        /// <summary>
        /// Connect two nodes, setting <paramref name="source"/> as parent of <paramref name="target"/> and <paramref name="target"/> as child of <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source node</param>
        /// <param name="target">The target node</param>
        /// <exception cref="ArgumentException">Thown if <paramref name="source"/> or <paramref name="target"/> values are unvalid.</exception>
        public void Connect(Node source, Node target)
        {
            if (!source.ChildType.IsAssignableFrom(target.GetType()))
                throw new ArgumentException($"ERROR: Source node child type({source.GetType()}) can handle target's type ({target.GetType()}) as a child. It should be {source.ChildType}");

            if (!_nodeSet.Contains(source) || !_nodeSet.Contains(target))
                throw new ArgumentException("ERROR: Source and/or target nodes are not in the graph.");                       // O(1) -> N = _nodeSet.Count()

            if (!source.CanAddAChild())
                throw new ArgumentException("ERROR: Maximum child count reached in source");

            if (!target.CanAddAParent())
                throw new ArgumentException("ERROR: Maximum parent count reached in target");

            if (!CanRepeatConnection && AreNodesDirectlyConnected(source, target))
                throw new ArgumentException("ERROR: Can't create two connections with the same source and target.");          // O(N) -> N = src.children.count(), tgt.parent.count()

            if (!CanCreateLoops && AreNodesConnected(target, source))
                throw new ArgumentException("ERROR: Can't create a loop in this graph.");                                     // O(N) -> N = Reachable nodes from target            

            source.Children.Add(target);
            target.Parents.Add(source);
        } 

        public virtual void Initialize()
        {
            Nodes.ForEach(node => node.Initialize());
        }

        /// <summary>
        /// Change the start node of the graph. <paramref name="node"/> must be contained by the graph.
        /// </summary>
        /// <param name="node">The new start node.</param>
        /// <returns>True if the <paramref name="node"/> was a valid parameter and wasn't the start node yet, False otherwise</returns>
        public virtual bool SetStartNode(Node node)
        {
            if (!Nodes.Contains(node) || node == StartNode) return false;

            Nodes.MoveAtFirst(node);
            return true;
        }

        /// <summary>
        /// Find a node in this graph by it's <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <returns>The node with the given <paramref name="name"/> in this graph.</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public Node FindNode(string name)
        {
            if (_nodeDict.TryGetValue(name, out Node node))
            {
                return node;
            }
            else
            {
                throw new KeyNotFoundException($"Node \"{name}\" doesn't exist in this graph");
            }
        }

        /// <summary>
        /// Find a node of type <typeparamref name="T"/> in this graph by it's <paramref name="name"/>
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="name">The name of the node.</param>
        /// <returns>The node with the given <paramref name="name"/> in this graph.</returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public T FindNode<T>(string name) where T : Node
        {
            if(_nodeDict.TryGetValue(name, out Node node))
            {
                if(node is T element)
                {
                    return element;
                }
                else
                {
                    throw new InvalidCastException($"Node \"{name}\" exists, but is not an instance of {typeof(T).FullName} class.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Node \"{name}\" doesn't exist in this graph.");
            }
        }

        /// <summary>
        /// Returns if graph has a connection path between a and b -> O(n).
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <returns>True if a path between the nodes exists.</returns>
        public bool AreNodesConnected(Node source, Node target)
        {
            if(source == target) return true;
            HashSet<Node> unvisitedNodes = new HashSet<Node>();
            HashSet<Node> visitedNodes = new HashSet<Node>();
            unvisitedNodes.Add(source);
            while(unvisitedNodes.Count > 0)
            {
                Node n = unvisitedNodes.First();
                unvisitedNodes.Remove(n);
                visitedNodes.Add(n);
                foreach(var child in n.Children)
                {
                    if (child == null) continue;

                    if (child == target)
                        return true;
                    if(!visitedNodes.Contains(child))
                        unvisitedNodes.Add(child);
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if a connection (source -> target) already exists.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <returns>True if the connection exists.</returns>
        public bool AreNodesDirectlyConnected(Node source, Node target)
        {
            // should be both true or false
            return source.IsParentOf(target) || target.IsChildOf(source);
        }

        #endregion   
    }
}
