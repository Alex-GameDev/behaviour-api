using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Serialization;
using BehaviourAPI.StateMachines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Testing
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void Test_Serialization_CreateBT()
        {
            var tree = new BehaviourTree();
            int i = 0;

            var builder = new BehaviourGraphBuilder(tree);
            var leaf1 = new LeafNode().SetAction(new FunctionalAction(() => i++, () => Status.Success));
            var leaf2 = new LeafNode().SetAction(new FunctionalAction(() => i++, () => Status.Success));
            var leaf3 = new LeafNode().SetAction(new FunctionalAction(() => i++, () => Status.Success));
            var seq = new SequencerNode();

            builder.AddNode(new NodeData(leaf1, new List<Node> { seq }, new List<Node>()));
            builder.AddNode(new NodeData(leaf2, new List<Node> { seq }, new List<Node>()));
            builder.AddNode(new NodeData(leaf3, new List<Node> { seq }, new List<Node>()));
            builder.AddNode(new NodeData(seq, new List<Node>(), new List<Node> { leaf1, leaf2, leaf3 }));

            builder.Build();

            Assert.AreEqual(4, tree.NodeCount);

            // Throws an error when adding a node with a wrong type
            Assert.ThrowsException<ArgumentException>(() => builder.AddNode(new NodeData(new ActionState(), new List<Node>(), new List<Node>())));
            
            // Throws an error at building if the connection number is wrong
            Assert.ThrowsException<ArgumentException>(() =>
            {
                builder.AddNode(new NodeData(new LeafNode(), new List<Node>(), new List<Node> { seq }));
                builder.Build();
            });

            tree.SetRootNode(seq);
            Assert.AreEqual(3, seq.ChildCount);
            Assert.AreEqual(1, leaf1.ParentCount);

            tree.Start();
            Assert.AreEqual(1, i);
            Assert.AreEqual(Status.Running, leaf1.Status);

            tree.Update();
            Assert.AreEqual(2, i);
            Assert.AreEqual(Status.Running, leaf2.Status);

            tree.Update();
            Assert.AreEqual(3, i);
            Assert.AreEqual(Status.Running, leaf3.Status);
        }
    }
}
