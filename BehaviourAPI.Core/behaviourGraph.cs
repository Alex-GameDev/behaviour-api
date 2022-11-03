namespace BehaviourAPI.Core
{
    public abstract class BehaviourGraph : IStatusHandler
    {
        #region ----------------------------------------- Properties -------------------------------------------

        /// <summary>
        /// The base type of the <see cref="Node"/> elements that this <see cref="BehaviourGraph"/> can contain.
        /// </summary>
        public abstract System.Type NodeType { get; }

        /// <summary>
        /// The base type of the <see cref="Connection"/> elements that this <see cref="BehaviourGraph"/> can contain.
        /// </summary>
        public abstract System.Type ConnectionType { get; }

        /// <summary>
        /// The default entry point of the graph
        /// </summary>
        public Node? StartNode => Nodes[0];

        /// <summary>
        /// The current execution status of the graph.
        /// </summary>
        public Status Status { get => _status; protected set => _status = value; }

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
        public List<Connection> Connections = new List<Connection>();
        public bool ExecuteOnLoop = true;

        // Used internally to find nodes by name
        protected Dictionary<string, Node> nodeDict = new Dictionary<string, Node>();

        Status _status;

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
                throw new DuplicateWaitObjectException(name, "This graph already contains a node with this name.");
            }            
        }

        protected T CreateConnection<T>(Node source, Node target) where T : Connection, new()
        {
            if (!Nodes.Contains(source) || !Nodes.Contains(target))
                throw new ArgumentException("ERROR: Source and/or target nodes are not in the graph.");

            if(!CanRepeatConnection && AreNodesDirectlyConnected(source, target))
                throw new ArgumentException("ERROR: Can't create two connections with the same source and target.");

            if (!CanCreateLoops && AreNodesConnected(target, source))
                throw new ArgumentException("ERROR: Can't create a loop in this graph.");

            T connection = new();
            connection.BehaviourGraph = this;
            connection.SourceNode = source;
            connection.TargetNode = target;

            target.InputConnections.Add(connection);
            source.OutputConnections.Add(connection);

            Connections.Add(connection);

            return connection;           
        }

        public Node CreateNode(Type type)
        {
            if (!type.IsSubclassOf(NodeType))
                throw new InvalidCastException("ERROR: \"type\" value is not a type derived from the graph node type.");

            Node? node = (Node?)Activator.CreateInstance(type);
            if(node!= null)
            {
                node.BehaviourGraph = this;
                Nodes.Add(node);
                return node;
            }
            throw new NullReferenceException("ERROR: Node couldn't be created.");
        }

        public Connection CreateConnection(Type type, Node source, Node target,
            int sourceIndex = -1, int targetIndex = -1)
        {
            if(type.IsSubclassOf(type))
                throw new InvalidCastException("ERROR: \"type\" value is not a type derived from the graph connection type.");

            if (!Nodes.Contains(source) || !Nodes.Contains(target))
                throw new ArgumentException("ERROR: Source and/or target nodes are not in the graph.");

            if (!CanRepeatConnection && AreNodesDirectlyConnected(source, target))
                throw new ArgumentException("ERROR: Can't create two connections with the same source and target.");

            if (!CanCreateLoops && AreNodesConnected(target, source))
                throw new ArgumentException("ERROR: Can't create a loop in this graph.");

            if (Nodes.Contains(source) && Nodes.Contains(target))
            {
                Connection? connection = (Connection?)Activator.CreateInstance(type);
                if(connection != null)
                {
                    connection.BehaviourGraph = this;
                    connection.SourceNode = source;
                    connection.TargetNode = target;

                    if (sourceIndex == -1) source.OutputConnections.Add(connection);
                    else source.OutputConnections.Insert(sourceIndex, connection);
                    if (targetIndex == -1) target.InputConnections.Add(connection);
                    else target.InputConnections.Insert(targetIndex, connection);

                    Connections.Add(connection);
                    return connection;
                }
                throw new NullReferenceException("ERROR: Connection couldn't be created.");
            }
            else
            {
                throw new ArgumentException("ERROR: Source and/or target nodes are not in the graph.");
            }
        }

        public void RemoveNode(Node node)
        {
            Nodes.Remove(node);
        }

        public void RemoveConnection(Connection connection)
        {
            connection.SourceNode?.OutputConnections.Remove(connection);
            connection.TargetNode?.InputConnections.Remove(connection);
            Connections.Remove(connection);
        }

        public virtual void Initialize()
        {
            Nodes.ForEach(node => node.Initialize());
            Connections.ForEach(conn => conn.Initialize());
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
                foreach(var child in n.GetChildNodes())
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

        #region --------------------------------------- Runtime Methods ----------------------------------------

        /// <summary>
        /// Enter this behavior graph
        /// </summary>
        public virtual void Start() => Status = Status.Running;

        /// <summary>
        /// Executes every frame
        /// </summary>

        public abstract void Update();

        /// <summary>
        /// Executes the last execution frame
        /// </summary>
        public virtual void Stop() => Status = Status.None;

        /// <summary>
        /// Finish the execution status giving 
        /// </summary>
        /// <param name="executionResult"></param>
        public void Finish(Status executionResult) => Status = executionResult;

        #endregion
    }
}
