namespace BehaviourAPI.BehaviourTrees
{
    using BehaviourAPI.Core.Exceptions;
    using Core;
    using Core.Actions;
    using Core.Perceptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Action = Core.Actions.Action;

    /// <summary>
    /// Decision system that consists of traversing a tree in depth depending on the result returned by its nodes.
    /// The execution methods are propagated along the tree from the root node.
    /// </summary>
    public class BehaviourTree : BehaviourGraph
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type NodeType => typeof(BTNode);

        public override bool CanRepeatConnection => false; // This won't happen because BTNode's max input connections is 1.

        public override bool CanCreateLoops => false;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        BTNode m_rootNode;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Create a new decorator node of type <typeparamref name="T"/> named <paramref name="name"/>  in this <see cref="BehaviourTree"/> that have <paramref name="child"/> as a child.
        /// </summary>
        /// <typeparam name="T">The type of decorator.</typeparam>
        /// <param name="name">The name of the decorator.</param>
        /// <param name="child">The child BT Node.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateDecorator<T>(string name, BTNode child) where T : DecoratorNode, new()
        {
            T node = CreateNode<T>(name);
            Connect(node, child);
            node.SetChild(child);
            return node;
        }

        /// <summary>
        /// Create a new decorator node of type <typeparamref name="T"/>  in this <see cref="BehaviourTree"/> that have <paramref name="child"/> as a child.
        /// </summary>
        /// <typeparam name="T">The type of decorator.</typeparam>
        /// <param name="child">The child BT Node.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateDecorator<T>(BTNode child) where T : DecoratorNode, new()
        {
            T node = CreateNode<T>();
            Connect(node, child);
            node.SetChild(child);
            return node;
        }

        /// <summary>
        /// Create a new composite node of type <typeparamref name="T"/> named <paramref name="name"/> in this <see cref="BehaviourTree"/> that have <paramref name="children"/> as children.
        /// </summary>
        /// <typeparam name="T">The type of the composite.</typeparam>
        /// <param name="name">The name of the composite.</param>
        /// <param name="children">The children of the composite.</param>
        /// <param name="randomOrder">true if the children will be executed in a diferent random order each time.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateComposite<T>(string name, List<BTNode> children, bool randomOrder = false) where T : CompositeNode, new()
        {
            T node = CreateNode<T>(name);
            node.IsRandomized = randomOrder;
            children.ForEach(child =>
            {
                if(child.Parents.Count == 0)
                {
                    Connect(node, child);
                    node.AddChild(child);
                }
            });
            return node;
        }

        /// <summary>
        /// Create a new composite node of type <typeparamref name="T"/> in this <see cref="BehaviourTree"/> that have <paramref name="children"/> as children.
        /// </summary>
        /// <typeparam name="T">The type of the composite.</typeparam>
        /// <param name="children">The children of the composite.</param>
        /// <param name="randomOrder">true if the children will be executed in a diferent random order each time.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateComposite<T>(List<BTNode> children, bool randomOrder = false) where T : CompositeNode, new()
        {
            T node = CreateNode<T>();
            node.IsRandomized = randomOrder;
            children.ForEach(child =>
            {
                if (child.Parents.Count == 0)
                {
                    Connect(node, child);
                    node.AddChild(child);
                }
            });
            return node;
        }

        /// <summary>
        /// Create a new composite node of type <typeparamref name="T"/> named <paramref name="name"/> in this <see cref="BehaviourTree"/> that have <paramref name="childs"/> as children.
        /// </summary>
        /// <typeparam name="T">The type of the composite.</typeparam>
        /// <param name="name">The name of the composite.</param>
        /// <param name="childs">The children of the composite.</param>
        /// <param name="randomOrder">true if the children will be executed in a diferent random order each time.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateComposite<T>(string name, bool randomOrder = false, params BTNode[] childs) where T : CompositeNode, new()
        {
            return CreateComposite<T>(name, childs.ToList(), randomOrder);
        }

        /// <summary>
        /// Create a new composite node of type <typeparamref name="T"/> in this <see cref="BehaviourTree"/> that have <paramref name="childs"/> as children.
        /// </summary>
        /// <typeparam name="T">The type of the composite.</typeparam>
        /// <param name="childs">The children of the composite.</param>
        /// <param name="randomOrder">true if the children will be executed in a diferent random order each time.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateComposite<T>(bool randomOrder = false, params BTNode[] childs) where T : CompositeNode, new()
        {
            return CreateComposite<T>(childs.ToList(), randomOrder);
        }

        /// <summary>
        /// Create a new <see cref="LeafNode"/> named <paramref name="name"/> in this <see cref="BehaviourTree"/> that executes the action specified in <paramref name="action"/>.
        /// </summary>
        /// <param name="name">The name of the leaf node.</param>
        /// <param name="action">The action that the leaf node executes.</param>
        /// <returns>The <see cref="LeafNode"/> created.</returns>
        public LeafNode CreateLeafNode(string name, Action action = null)
        {
            LeafNode node = CreateNode<LeafNode>(name);
            node.Action = action;
            return node;
        }

        /// <summary>
        /// Create a new <see cref="LeafNode"/> in this <see cref="BehaviourTree"/> that executes the action specified in <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action that the leaf node executes.</param>
        /// <returns>The <see cref="LeafNode"/> created.</returns>
        public LeafNode CreateLeafNode(Action action = null)
        {
            LeafNode node = CreateNode<LeafNode>();
            node.Action = action;
            return node;
        }

        public override bool SetStartNode(Node node)
        {
            bool starNodeUpdated = base.SetStartNode(node);
            if(starNodeUpdated) m_rootNode = node as BTNode;
            return starNodeUpdated;
        }
        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            if (Nodes.Count == 0)
                throw new EmptyGraphException(this);
            m_rootNode = StartNode as BTNode;
            m_rootNode?.Start();
        }

        public override void Execute()
        {
            m_rootNode?.Update();
            Status = m_rootNode?.Status ?? Status.Error;
        }

        public override void Stop()
        {
            base.Stop();
            m_rootNode?.Stop();
        }

        #endregion
    }
}
