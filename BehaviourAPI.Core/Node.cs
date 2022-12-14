using System;
using System.Collections.Generic;

namespace BehaviourAPI.Core
{
    public abstract class Node
    {

        #region ------------------------------------------ Properties -----------------------------------------

        public BehaviourGraph BehaviourGraph { get; set; }

        /// <summary>
        /// The type of nodes that this node can handle as a child(s).
        /// </summary>
        public abstract Type ChildType { get; }

        /// <summary>
        /// Maximum number of elements in <see cref="Parents"/>.
        /// </summary>
        public abstract int MaxInputConnections { get; }

        /// <summary>
        /// Maximum number of elements in <see cref="Children"/>.
        /// </summary>
        public abstract int MaxOutputConnections { get; }

        /// <summary>
        /// List of connections in the graph with this node as target.
        /// </summary>
        internal List<Node> Parents;

        /// <summary>
        /// List of connections in the graph with this node as source.
        /// </summary>
        internal List<Node> Children;

        public int ChildCount => Children.Count;

        public int ParentCount => Parents.Count;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Node()
        {
            Children = new List<Node>();
            Parents = new List<Node>();
        }

        public Node GetChildAt(int index) => Children[index];

        public Node GetParentAt(int index) => Parents[index];

        /// <summary>
        /// Checks if a node is connected with this as target.
        /// </summary>
        /// <param name="node">The checked target node</param>
        /// <returns>True if the checked node is a child of this node.</returns>
        public bool IsChildOf(Node node)
        {
            return Parents.Contains(node);
        }

        /// <summary>
        /// Checks if a node is connected with this as source.
        /// </summary>
        /// <param name="node">The checked source node</param>
        /// <returns>True if the checked node is a parent of this node.</returns>
        public bool IsParentOf(Node node)
        {
            return Children.Contains(node);
        }

        public Node GetFirstChild() => Children.Count > 0 ? Children[0] : null;

        public Node GetFirstParent() => Parents.Count > 0 ? Parents[0] : null;

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

        /// <summary>
        /// Check if the node can have more children.
        /// </summary>
        /// <returns>True if can have more children, false otherwise.</returns>
        public bool CanAddAChild() => MaxOutputConnections == -1 || Children.Count < MaxOutputConnections;

        /// <summary>
        /// Check if the node can have more parents.
        /// </summary>
        /// <returns>True if can have more children, false otherwise.</returns>
        public bool CanAddAParent() => MaxInputConnections == -1 || Parents.Count < MaxInputConnections;

        /// <summary>
        /// Build the internal connection references.
        /// </summary>
        protected internal virtual void BuildConnections(List<Node> parents, List<Node> children)
        {
            if (MaxInputConnections != -1 && parents.Count > MaxInputConnections)
                throw new ArgumentException();

            if (MaxOutputConnections != -1 && children.Count > MaxOutputConnections)
                throw new ArgumentException();

            Parents = parents;
            Children = children;
        }

        #endregion
    }
}
