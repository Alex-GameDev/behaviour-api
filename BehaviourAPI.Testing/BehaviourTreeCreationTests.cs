namespace BehaviourAPI.Testing
{
    using BehaviourTrees;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BehaviourTreeCreationTests
    {
        [TestMethod("Composite node connections")]
        public void Test_BT_CompositeNode_Connections()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateLeafNode();
            var action_2 = tree.CreateLeafNode();
            var action_3 = tree.CreateLeafNode();
            var composite = tree.CreateComposite<SelectorNode>(false, action_1, action_2, action_3);

            Assert.AreEqual(4, tree.Nodes.Count);

            Assert.AreEqual(3, composite.Children.Count);
            Assert.AreEqual(0, composite.Parents.Count);
            Assert.AreEqual(1, action_2.Parents.Count);
            Assert.AreEqual(0, action_2.Children.Count);

            Assert.AreEqual(true, action_2.IsChildOf(composite));
            Assert.AreEqual(true, composite.IsParentOf(action_2));

            float f = float.MaxValue * 1.3f;
            Assert.AreEqual(true, f > float.MaxValue);
        }

        [TestMethod("Decorator node connections")]
        public void Test_BT_DecoratorNode_Connections()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateLeafNode();
            var decorator = tree.CreateDecorator<InverterNode>(action_1);

            Assert.AreEqual(1, decorator.Children.Count);
            Assert.AreEqual(0, decorator.Parents.Count);

            Assert.AreEqual(1, action_1.Parents.Count);
            Assert.AreEqual(0, action_1.Children.Count);

            Assert.AreEqual(true, action_1.IsChildOf(decorator));
            Assert.AreEqual(true, decorator.IsParentOf(action_1));
        }

        [TestMethod("Start node")]
        public void Test_BT_StartNode()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateLeafNode();
            var decorator = tree.CreateDecorator<InverterNode>(action_1);
            tree.SetStartNode(decorator);

            Assert.AreEqual(decorator, tree.StartNode);
            Assert.AreEqual(decorator, tree.Nodes[0]);
        }

        [TestMethod("Complex tree connections")]
        public void Test_BT_MultilevelTree_Connections()
        {
            BehaviourTree tree = new BehaviourTree();

            var action_1 = tree.CreateLeafNode();
            var action_2 = tree.CreateLeafNode();
            var action_3 = tree.CreateLeafNode();
            var composite_1 = tree.CreateComposite<SelectorNode>(false, action_1, action_2, action_3);

            var action_4 = tree.CreateLeafNode();
            var action_5 = tree.CreateLeafNode();
            var action_6 = tree.CreateLeafNode();
            var composite_2 = tree.CreateComposite<SelectorNode>(false, action_2, action_3, action_4, action_5, action_6);

            var action_7 = tree.CreateLeafNode();

            var composite_root = tree.CreateComposite<SequencerNode>(false, composite_1, composite_2, action_7);

            Assert.AreEqual(10, tree.Nodes.Count);

            Assert.AreEqual(3, composite_1.Children.Count);
            Assert.AreEqual(1, composite_1.Parents.Count);
            Assert.AreEqual(3, composite_root.Children.Count);
            Assert.AreEqual(0, composite_root.Parents.Count);

            Assert.AreEqual(true, tree.SetStartNode(composite_root));
            Assert.AreEqual(false, tree.SetStartNode(composite_root));

            var decorator = tree.CreateDecorator<InverterNode>(action_1);
            Assert.AreEqual(0, decorator.Children.Count);
            Assert.AreEqual(1, action_1.Parents.Count);

            tree.Start();
            tree.Update();
        }
    }
}