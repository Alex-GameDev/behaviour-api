namespace BehaviourAPI.Testing
{
    using BehaviourAPI.Core.Exceptions;
    using BehaviourTrees;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class BehaviourTreeCreationTests
    {
        [TestMethod]

        public void Test_BT_LeafNode_Connections()
        {
            BehaviourTree tree = new BehaviourTree();
            var action1 = tree.CreateLeafNode();
            var decorator = tree.CreateDecorator<SuccederNode>(action1);
            Assert.AreEqual(1, action1.Parents.Count);
            Assert.AreEqual(0, action1.Children.Count);
            Assert.ThrowsException<ArgumentException>(() => tree.CreateDecorator<SuccederNode>(action1));       // 1 parent max
            Assert.ThrowsException<ArgumentException>(() => tree.Connect(action1, decorator));                  // 0 child max
            Assert.AreEqual(0, decorator.Parents.Count);
            Assert.AreEqual(0, action1.Children.Count);
        }

        [TestMethod]
        public void Test_BT_Decorator_Connections()
        {
            BehaviourTree tree = new BehaviourTree();
            var action1 = tree.CreateLeafNode();
            var decorator = tree.CreateDecorator<SuccederNode>(action1);
            var root_decorator = tree.CreateDecorator<SuccederNode>(decorator);

            Assert.AreEqual(1, decorator.Parents.Count);
            Assert.AreEqual(1, decorator.Children.Count);
            Assert.ThrowsException<ArgumentException>(() => tree.CreateDecorator<SuccederNode>(decorator));     // 1 parent max
            Assert.ThrowsException<ArgumentException>(() => tree.Connect(decorator, root_decorator));           // 1 child max            
            Assert.AreEqual(1, decorator.Parents.Count);
            Assert.AreEqual(1, decorator.Children.Count);
            Assert.AreEqual(1, root_decorator.Children.Count);
        }

        [TestMethod]
        public void Test_BT_CompositeConnections()
        {
            BehaviourTree tree = new BehaviourTree();
            var action1 = tree.CreateLeafNode();
            var action2 = tree.CreateLeafNode();
            var composite = tree.CreateComposite<SequencerNode>(false, action1, action2);
            var root_decorator = tree.CreateDecorator<SuccederNode>(composite);

            Assert.AreEqual(1, composite.Parents.Count);
            Assert.AreEqual(2, composite.Children.Count);
            Assert.ThrowsException<ArgumentException>(() => tree.CreateDecorator<SuccederNode>(composite));     // 1 parent max
            Assert.ThrowsException<ArgumentException>(() => tree.Connect(composite, action2));                  // Cannot repeat connection
            Assert.AreEqual(1, composite.Parents.Count);
            Assert.AreEqual(2, composite.Children.Count);

            var action3 = tree.CreateLeafNode();
            tree.Connect(composite, action3);                       // (!) "child" reference not created
            Assert.AreEqual(3, composite.Children.Count);
        }


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

            Assert.ThrowsException<MissingActionException>(tree.Start);
        }
    }
}