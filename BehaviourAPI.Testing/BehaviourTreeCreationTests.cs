namespace BehaviourAPI.Testing
{
    using BehaviourAPI.Core.Actions;
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
            Assert.AreEqual(true, action1.IsChildOf(decorator));
            Assert.AreEqual(true, decorator.IsParentOf(action1));
            Assert.ThrowsException<ArgumentException>(() => tree.CreateDecorator<SuccederNode>(action1));       
            Assert.ThrowsException<ArgumentException>(() => tree.CreateComposite<SequencerNode>(false, action1));  
            Assert.ThrowsException<ArgumentException>(() => tree.Connect(action1, decorator));                 
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
            Assert.AreEqual(true, decorator.IsChildOf(root_decorator));
            Assert.AreEqual(true, root_decorator.IsParentOf(decorator));
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
            Assert.AreEqual(true, action1.IsChildOf(composite));
            Assert.AreEqual(true, composite.IsParentOf(action1));
            Assert.AreEqual(true, composite.IsChildOf(root_decorator));
            Assert.AreEqual(true, root_decorator.IsParentOf(composite));
            Assert.ThrowsException<ArgumentException>(() => tree.CreateDecorator<SuccederNode>(composite));     // 1 parent max
            Assert.ThrowsException<ArgumentException>(() => tree.Connect(composite, action2));                  // Cannot repeat connection
            Assert.AreEqual(1, composite.Parents.Count);
            Assert.AreEqual(2, composite.Children.Count);

            var action3 = tree.CreateLeafNode();
            tree.Connect(composite, action3);                       // (!) "child" reference not created
            Assert.AreEqual(3, composite.Children.Count);
        }

        [TestMethod]
        public void Test_BT_BehaviourTree_Nodes()
        {
            BehaviourTree tree = new BehaviourTree();

            Assert.ThrowsException<EmptyGraphException>(() => tree.StartNode);

            var action_1 = tree.CreateLeafNode();
            var action_2 = tree.CreateLeafNode();
            var action_3 = tree.CreateLeafNode();
            var composite_1 = tree.CreateComposite<SelectorNode>(false, action_1, action_2, action_3);

            var action_4 = tree.CreateLeafNode();
            var action_5 = tree.CreateLeafNode();
            var action_6 = tree.CreateLeafNode();
            var decorator = tree.CreateDecorator<SuccederNode>(action_5);
            var composite_2 = tree.CreateComposite<SelectorNode>(false, action_4, decorator, action_6);

            var action_7 = tree.CreateLeafNode();

            var composite_root = tree.CreateComposite<SequencerNode>(false, composite_1, composite_2, action_7);

            Assert.AreEqual(11, tree.Nodes.Count);
            Assert.AreEqual(action_1, tree.StartNode);
            
            var external_node = new BehaviourTree().CreateLeafNode();
            Assert.IsFalse(tree.SetStartNode(external_node));  // Is not in the graph
            Assert.AreEqual(action_1, tree.StartNode);
            Assert.IsTrue(tree.SetStartNode(composite_root));   
            Assert.IsFalse(tree.SetStartNode(composite_root)); // Is currently the start node
            Assert.AreEqual(composite_root, tree.StartNode);
        }
    }
}