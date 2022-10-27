namespace BehaviourAPI.Core
{
    public abstract class BehaviourEngine : IStatusHandler
    {
        #region ----------------------------------------- Properties -------------------------------------------

        /// <summary>
        /// The base type of the <see cref="Node"/> elements that this <see cref="BehaviourEngine"/> can contain.
        /// </summary>
        public abstract System.Type NodeType { get; }

        /// <summary>
        /// The base type of the <see cref="Connection"/> elements that this <see cref="BehaviourEngine"/> can contain.
        /// </summary>
        public abstract System.Type ConnectionType { get; }

        /// <summary>
        /// The default entry point of the graph
        /// </summary>
        public Node? StartNode => Nodes[0];

        /// <summary>
        /// The current execution status of the graph.
        /// </summary>
        public Status Status { get; protected set; }

        #endregion

        #region ------------------------------------------- Fields ---------------------------------------------

        public List<Node> Nodes = new List<Node>();
        public List<Connection> Connections = new List<Connection>();
        public bool ExecuteOnLoop = true;

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
        public abstract void Start();

        /// <summary>
        /// Executes every frame
        /// </summary>

        public abstract void Update();

        /// <summary>
        /// Executes the last execution frame
        /// </summary>
        public abstract void Stop();

        #endregion
    }
}