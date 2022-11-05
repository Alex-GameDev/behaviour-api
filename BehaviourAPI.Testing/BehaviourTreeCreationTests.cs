namespace BehaviourAPI.Testing
{
    using BehaviourTrees;

    [TestClass]
    public class BehaviourTreeCreationTests
    {
        [TestMethod("Composite node connections")]
        public void Test_BT_CompositeNode_Connections()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateActionBTNode("Nodo 1");
            var action_2 = tree.CreateActionBTNode("Nodo 2");
            var action_3 = tree.CreateActionBTNode("Nodo 3");
            var composite = tree.CreateComposite<SelectorNode>("Sel", false, action_1, action_2, action_3);

            Assert.AreEqual(4, tree.Nodes.Count);
            Assert.AreEqual(3, tree.Connections.Count);

            Assert.AreEqual(3, composite.GetChildNodes().Count());
            Assert.AreEqual(0, composite.GetParentNodes().Count());
            Assert.AreEqual(3, composite.OutputConnections.Count);
            Assert.AreEqual(0, composite.InputConnections.Count);
            Assert.AreEqual(1, action_2.GetParentNodes().Count());
            Assert.AreEqual(1, action_2.InputConnections.Count);
            Assert.AreEqual(0, action_2.OutputConnections.Count);

            Assert.AreEqual(true, action_2.IsChildOf(composite));
            Assert.AreEqual(true, composite.IsParentOf(action_2));

            float f = float.MaxValue * 1.3f;
            Assert.AreEqual(true, f > float.MaxValue);
        }

        [TestMethod("Decorator node connections")]
        public void Test_BT_DecoratorNode_Connections()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateActionBTNode("Nodo 1");
            var decorator = tree.CreateDecorator<InverterNode>("Sel", action_1);

            Assert.AreEqual(1, decorator.GetChildNodes().Count());
            Assert.AreEqual(0, decorator.GetParentNodes().Count());
            Assert.AreEqual(1, decorator.OutputConnections.Count);
            Assert.AreEqual(0, decorator.InputConnections.Count);

            Assert.AreEqual(0, action_1.GetChildNodes().Count());
            Assert.AreEqual(1, action_1.GetParentNodes().Count());
            Assert.AreEqual(1, action_1.InputConnections.Count);
            Assert.AreEqual(0, action_1.OutputConnections.Count);

            Assert.AreEqual(true, action_1.IsChildOf(decorator));
            Assert.AreEqual(true, decorator.IsParentOf(action_1));
        }

        [TestMethod("Start node")]
        public void Test_BT_StartNode()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateActionBTNode("Nodo 1");
            var decorator = tree.CreateDecorator<InverterNode>("Sel", action_1);
            tree.SetStartNode(decorator);

            Assert.AreEqual(decorator, tree.StartNode);
            Assert.AreEqual(decorator, tree.Nodes[0]);
        }

        [TestMethod("Complex tree connections")]
        public void Test_BT_MultilevelTree_Connections()
        {
            BehaviourTree tree = new BehaviourTree();

            var action_1 = tree.CreateActionBTNode("Nodo 1");
            var action_2 = tree.CreateActionBTNode("Nodo 2");
            var action_3 = tree.CreateActionBTNode("Nodo 3");
            var composite_1 = tree.CreateComposite<SelectorNode>("Sel_1", false, action_1, action_2, action_3);

            var action_4 = tree.CreateActionBTNode("Nodo 4");
            var action_5 = tree.CreateActionBTNode("Nodo 5");
            var action_6 = tree.CreateActionBTNode("Nodo 6");
            var composite_2 = tree.CreateComposite<SelectorNode>("Sel_2", false, action_2, action_3, action_4, action_5, action_6);

            var action_7 = tree.CreateActionBTNode("Nodo 7");

            var composite_root = tree.CreateComposite<SequencerNode>("Seq", false, composite_1, composite_2, action_7);

            Assert.AreEqual(10, tree.Nodes.Count);
            Assert.AreEqual(9, tree.Connections.Count);

            Assert.AreEqual(3, composite_1.OutputConnections.Count);
            Assert.AreEqual(1, composite_1.InputConnections.Count);
            Assert.AreEqual(3, composite_root.OutputConnections.Count);
            Assert.AreEqual(0, composite_root.InputConnections.Count);

            Assert.AreEqual(false, tree.SetStartNode(action_7));
            Assert.AreEqual(true, tree.SetStartNode(composite_root));
            Assert.AreEqual(false, tree.SetStartNode(composite_root));

            var decorator = tree.CreateDecorator<InverterNode>("Inv", action_1);
            Assert.AreEqual(0, decorator.GetChildNodes().Count());
            Assert.AreEqual(1, action_1.GetParentNodes().Count());

            tree.Start();
            tree.Update();
        }
    }
}