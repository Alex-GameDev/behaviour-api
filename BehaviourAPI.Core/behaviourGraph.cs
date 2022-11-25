namespace BehaviourAPI.Core
{
    public abstract class BehaviourGraph : BehaviourSystem
    {
        #region ----------------------------------------- Properties -------------------------------------------

        /// <summary>
        /// The base type of the <see cref="Node"/> elements that this <see cref="BehaviourGraph"/> can contain.
        /// </summary>
        public abstract System.Type NodeType { get; }

        /// <summary>
        /// The default entry point of the graph
        /// </summary>
        public Node? StartNode => Nodes[0];

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
        protected Dictionary<string, Node> nodeDict = new Dictionary<string, Node>();

        #endregion

        #region ---------------------------------------- Build methods -----------------------------------------

        protected T CreateNode<T>(string name) where T : Node, new()
        {
            T node = new();
            node.BehaviourGraph = this;
            node.Name = name;
            if(nodeDict.TryAdd(name, node))
            {
                Nodes.Add(node);
                return node;
            }
            else
            {
                throw new ArgumentException(name, "This graph already contains a node with this name.");
            }            
        }

        /// <summary>
        /// Connect two nodes
        /// </summary>
        /// <param name="source">The source node</param>
        /// <param name="target"> The target node</param>
        /// <exception cref="ArgumentException">Thown when some of the nodes are unvalid.</exception>
        public void Connect(Node source, Node target)
        {
            if (!source.ChildType.IsAssignableFrom(target.GetType()))
                throw new ArgumentException($"ERROR: Source node child type({source.ChildType}) can handle target's type ({target.GetType()}) as a child.");

            if (!Nodes.Contains(source) || !Nodes.Contains(target))
                throw new ArgumentException("ERROR: Source and/or target nodes are not in the graph.");

            if (!CanRepeatConnection && AreNodesDirectlyConnected(source, target))
                throw new ArgumentException("ERROR: Can't create two connections with the same source and target.");

            if (!CanCreateLoops && AreNodesConnected(target, source))
                throw new ArgumentException("ERROR: Can't create a loop in this graph.");

            source.Children.Add(target);
            target.Parents.Add(source);
        }

        /// <summary>
        /// Disconnect two nodes
        /// </summary>
        /// <param name="source">The source node</param>
        /// <param name="target"> The target node</param>
        /// <exception cref="ArgumentException">Thown when some of the nodes are unvalid.</exception>
        public void Disconnect(Node source, Node target)
        {
            if (!Nodes.Contains(source) || !Nodes.Contains(target))
                throw new ArgumentException("ERROR: Source and/or target nodes are not in the graph.");

            if(source.IsParentOf(target) && target.IsChildOf(source))
            {
                source.Children.Remove(target);
                target.Parents.Remove(source);
            }
        }

        public Node CreateNode(Type type)
        {
            if (!type.IsSubclassOf(NodeType))
                throw new InvalidCastException("ERROR: \"type\" value is not a type derived from the graph node type.");

            Node? node = (Node?)Activator.CreateInstance(type);
            if(node != null)
            {
                node.BehaviourGraph = this;
                Nodes.Add(node);
                return node;
            }
            throw new NullReferenceException("ERROR: Node couldn't be created.");
        }

        public void RemoveNode(Node node)
        {
            Nodes.Remove(node);
        }

        public virtual void Initialize()
        {
            Nodes.ForEach(node => node.Initialize());
        }

        /// <summary>
        /// For serialization reasons, start node must be always the first node in the list
        /// </summary>
        public virtual bool SetStartNode(Node node)
        {
            if (!Nodes.Contains(node) || node == StartNode) return false;

            Nodes.MoveAtFirst(node);
            return true;
        }

        public Node? FindNode(string name)
        {
            if (nodeDict.TryGetValue(name, out Node? node))
            {
                return node;
            }
            else
            {
                throw new KeyNotFoundException($"Node \"{name}\" doesn't exist in this graph");
            }
        }

        public T? FindNode<T>(string name) where T : Node
        {
            if(nodeDict.TryGetValue(name, out Node? node))
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
                throw new KeyNotFoundException($"Node \"{name}\" doesn't exist in this graph");
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
            return source.IsConnectedWith(target);
        }

        #endregion   
    }
}
