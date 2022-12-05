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
            Assert.AreEqual(1, action1.ParentCount);
            Assert.AreEqual(0, action1.ChildCount);
            Assert.AreEqual(true, action1.IsChildOf(decorator));
            Assert.AreEqual(true, decorator.IsParentOf(action1));
            Assert.ThrowsException<ArgumentException>(() => tree.CreateDecorator<SuccederNode>(action1));       
            Assert.ThrowsException<ArgumentException>(() => tree.CreateComposite<SequencerNode>(false, action1));              
            Assert.AreEqual(0, decorator.ParentCount);
            Assert.AreEqual(0, action1.ChildCount);
        }

        [TestMethod]
        public void Test_BT_Decorator_Connections()
        {
            BehaviourTree tree = new BehaviourTree();
            var action1 = tree.CreateLeafNode();
            var decorator = tree.CreateDecorator<SuccederNode>(action1);
            var root_decorator = tree.CreateDecorator<SuccederNode>(decorator);

            Assert.AreEqual(1, decorator.ParentCount);
            Assert.AreEqual(1, decorator.ChildCount);
            Assert.AreEqual(true, decorator.IsChildOf(root_decorator));
            Assert.AreEqual(true, root_decorator.IsParentOf(decorator));
            Assert.ThrowsException<ArgumentException>(() => tree.CreateDecorator<SuccederNode>(decorator));     // 1 parent max
    
            Assert.AreEqual(1, decorator.ParentCount);
            Assert.AreEqual(1, decorator.ChildCount);
            Assert.AreEqual(1, root_decorator.ChildCount);
        }

        [TestMethod]
        public void Test_BT_CompositeConnections()
        {
            BehaviourTree tree = new BehaviourTree();
            var action1 = tree.CreateLeafNode();
            var action2 = tree.CreateLeafNode();
            var composite = tree.CreateComposite<SequencerNode>(false, action1, action2);
            var root_decorator = tree.CreateDecorator<SuccederNode>(composite);

            Assert.AreEqual(1, composite.ParentCount);
            Assert.AreEqual(2, composite.ChildCount);
            Assert.AreEqual(true, action1.IsChildOf(composite));
            Assert.AreEqual(true, composite.IsParentOf(action1));
            Assert.AreEqual(true, composite.IsChildOf(root_decorator));
            Assert.AreEqual(true, root_decorator.IsParentOf(composite));
            Assert.ThrowsException<ArgumentException>(() => tree.CreateDecorator<SuccederNode>(composite));     // 1 parent max
            Assert.AreEqual(1, composite.ParentCount);
            Assert.AreEqual(2, composite.ChildCount);
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

            Assert.AreEqual(11, tree.NodeCount);
            Assert.AreEqual(action_1, tree.StartNode);
            
            var external_node = new BehaviourTree().CreateLeafNode();
            Assert.ThrowsException<ArgumentException>(() => tree.SetRootNode(external_node));
            Assert.AreEqual(action_1, tree.StartNode);
            tree.SetRootNode(composite_root);   
            Assert.AreEqual(composite_root, tree.StartNode);
        }

        [TestMethod]
        public void Test_Build()
        {
            BehaviourTree tree = new BehaviourTree();
        }
    }
}