namespace BehaviourAPI.Core
{
    public abstract class Node : GraphElement
    {

        #region ------------------------------------------ Properties -----------------------------------------

        /// <summary>
        /// The type of nodes that this node can handle as a child(s).
        /// </summary>
        public abstract Type ChildType { get; }

        /// <summary>
        /// Maximum number of elements in <see cref="InputConnections"/>.
        /// </summary>
        public abstract int MaxInputConnections { get; }

        /// <summary>
        /// Maximum number of elements in <see cref="OutputConnections"/>.
        /// </summary>
        public abstract int MaxOutputConnections { get; }

        /// <summary>
        /// List of connections in the graph with this node as target.
        /// </summary>
        public List<Connection> InputConnections { get; private set; }

        /// <summary>
        /// List of connections in the graph with this node as source.
        /// </summary>
        public List<Connection> OutputConnections { get; private set; }

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Node()
        {
            OutputConnections = new List<Connection>();
            InputConnections = new List<Connection>();
        }

        /// <summary>
        /// Get all nodes connected with this as source.
        /// </summary>
        /// <returns>List of parent nodes</returns>
        public IEnumerable<Node?> GetParentNodes()
        {
            return InputConnections.Select((x) => x.SourceNode).Distinct();
        }

        /// <summary>
        /// Get all nodes connected with this as target.
        /// </summary>
        /// <returns>List of child nodes</returns>
        public IEnumerable<Node?> GetChildNodes()
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
        /// Return true if this node is the start node of the graph.
        /// </summary>
        public bool IsStartNode() => BehaviourGraph?.StartNode == this;

        #endregion
    }
}
