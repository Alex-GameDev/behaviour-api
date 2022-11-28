namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    using Core.Actions;
    using Core.Perceptions;
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

        BTNode? m_rootNode;
        
        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------
        public T CreateDecorator<T>(string name, BTNode child) where T : DecoratorNode, new()
        {
            T node = CreateNode<T>(name);
            if (child.Parents.Count == 0)
            {
                Connect(node, child);
                node.SetChild(child);
            }
            return node;
        }

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

        public T CreateComposite<T>(string name, bool randomOrder = false, params BTNode[] childs) where T : CompositeNode, new()
        {
            return CreateComposite<T>(name, childs.ToList(), randomOrder);
        }

        public LeafNode CreateLeafNode(string name, Action? action = null)
        {
            LeafNode node = CreateNode<LeafNode>(name);
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
