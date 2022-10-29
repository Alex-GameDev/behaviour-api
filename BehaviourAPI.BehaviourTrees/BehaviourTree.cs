namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    /// <summary>
    /// Decision system that consists of traversing a tree in depth depending on the result returned by its nodes.
    /// The execution methods are propagated along the tree from the root node.
    /// </summary>
    public class BehaviourTree : BehaviourGraph
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type NodeType => typeof(BTNode);
        public override Type ConnectionType => typeof(BTConnection);

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        BTNode? m_rootNode;
        
        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------
        public T CreateDecorator<T>(string name, BTNode child) where T : DecoratorNode, new()
        {
            T node = CreateNode<T>(name);
            if (child.InputConnections.Count == 0)
            {
                CreateConnection<BTConnection>(node, child);
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
                if(child.InputConnections.Count == 0)
                {
                    CreateConnection<BTConnection>(node, child);
                    node.AddChild(child);
                }
            });
            return node;
        }

        public T CreateComposite<T>(string name, bool randomOrder = false, params BTNode[] childs) where T : CompositeNode, new()
        {
            return CreateComposite<T>(name, childs.ToList(), randomOrder);
        }

        public T CreateLeafNode<T>(string name) where T : LeafNode, new()
        {
            T node = CreateNode<T>(name);
            node.Name = name;
            return node;
        }

        public override bool SetStartNode(Node node)
        {
            if (node.InputConnections.Count > 0) return false;
            bool starNodeUpdated = base.SetStartNode(node);
            if(starNodeUpdated) m_rootNode = node as BTNode;
            return starNodeUpdated;
        }
        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            m_rootNode = StartNode as BTNode;
            m_rootNode?.Start();
        }

        public override void Update()
        {
            m_rootNode?.Update();
            Status = m_rootNode?.Status ?? Status.Error;
        }

        public override void Stop()
        {
            m_rootNode?.Stop();
        }

        #endregion
    }
}
