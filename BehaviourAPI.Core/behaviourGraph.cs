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

        #endregion

        #region ------------------------------------------- Fields ---------------------------------------------

        public List<Node> Nodes = new List<Node>();
        public List<Connection> Connections = new List<Connection>();
        public bool ExecuteOnLoop = true;

        Status _status;

        #endregion

        #region ---------------------------------------- Build methods -----------------------------------------

        protected T CreateNode<T>(string name) where T : Node, new()
        {
            T node = new();
            node.BehaviourGraph = this;
            node.Name = name;
            Nodes.Add(node);
            return node;
        }

        protected T CreateConnection<T>(Node source, Node target) where T : Connection, new()
        {
            if(Nodes.Contains(source) && Nodes.Contains(target))
            {
                T connection = new();
                connection.BehaviourGraph = this;
                connection.SourceNode = source;
                connection.TargetNode = target;

                target.InputConnections.Add(connection);
                source.OutputConnections.Add(connection);

                Connections.Add(connection);

                return connection;
            }
            else
            {
                throw new Exception();
            }            
        }

        public Node? CreateNode(Type type)
        {
            if (!type.IsSubclassOf(NodeType)) return null;

            Node? node = (Node?)Activator.CreateInstance(type);
            if(node!= null)
            {
                node.BehaviourGraph = this;
            }
            return node;
        }

        public Connection? CreateConnection(Type type, Node source, Node target,
            int sourceIndex = -1, int targetIndex = -1)
        {
            if (Nodes.Contains(source) && Nodes.Contains(target) && type.IsSubclassOf(type))
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
                }
                return connection;
            }
            else
            {
                return null;
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

        #endregion   

        #region --------------------------------------- Runtime Methods ----------------------------------------

        /// <summary>
        /// Enter this behavior graph from a subgraph node or a <see cref="BehaviourRunner"/>
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
