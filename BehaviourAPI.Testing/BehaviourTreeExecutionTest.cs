using BehaviourAPI.BehaviourTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPITesting
{
    using BehaviourAPI.Core;
    [TestClass]
    public class BehaviourTreeExecutionTest
    {
        [TestMethod("Sequencer Execution Sucess")]
        public void TestSequencerExecutionSucess()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateLeafNode<ActionBTNode>("Nodo 1").SetAction((st) => Status.Sucess);
            var action_2 = tree.CreateLeafNode<ActionBTNode>("Nodo 2").SetAction((st) => Status.Sucess);
            var action_3 = tree.CreateLeafNode<ActionBTNode>("Nodo 3").SetAction((st) => Status.Sucess);
            var seq = tree.CreateComposite<SequencerNode>("Seq", false, action_1, action_2, action_3);
            tree.SetStartNode(seq);

            tree.Start();
            Assert.AreEqual(Status.Running, seq.Status);
            Assert.AreEqual(Status.Running, action_1.Status);
            Assert.AreEqual(Status.None, action_2.Status);
            Assert.AreEqual(Status.None, action_3.Status);

            tree.Update();
            Assert.AreEqual(Status.Running, seq.Status);
            Assert.AreEqual(Status.Sucess, action_1.Status);
            Assert.AreEqual(Status.Running, action_2.Status);
            Assert.AreEqual(Status.None, action_3.Status);

            tree.Update();
            Assert.AreEqual(Status.Running, seq.Status);
            Assert.AreEqual(Status.Sucess, action_1.Status);
            Assert.AreEqual(Status.Sucess, action_2.Status);
            Assert.AreEqual(Status.Running, action_3.Status);

            tree.Update();
            Assert.AreEqual(Status.Sucess, seq.Status);
            Assert.AreEqual(Status.Sucess, action_1.Status);
            Assert.AreEqual(Status.Sucess, action_2.Status);
            Assert.AreEqual(Status.Sucess, action_3.Status);
        }


        [TestMethod("Sequencer Execution Failure")]
        public void TestSequencerExecutionFailure()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateLeafNode<ActionBTNode>("Nodo 1").SetAction((st) => Status.Sucess);
            var action_2 = tree.CreateLeafNode<ActionBTNode>("Nodo 2").SetAction((st) => Status.Failure);
            var action_3 = tree.CreateLeafNode<ActionBTNode>("Nodo 3").SetAction((st) => Status.Sucess);
            var seq = tree.CreateComposite<SequencerNode>("Seq", false, action_1, action_2, action_3);
            tree.SetStartNode(seq);

            tree.Start();
            Assert.AreEqual(Status.Running, seq.Status);
            Assert.AreEqual(Status.Running, action_1.Status);
            Assert.AreEqual(Status.None, action_2.Status);
            Assert.AreEqual(Status.None, action_3.Status);

            tree.Update();
            Assert.AreEqual(Status.Running, seq.Status);
            Assert.AreEqual(Status.Sucess, action_1.Status);
            Assert.AreEqual(Status.Running, action_2.Status);
            Assert.AreEqual(Status.None, action_3.Status);

            tree.Update();
            Assert.AreEqual(Status.Failure, seq.Status);
            Assert.AreEqual(Status.Sucess, action_1.Status);
            Assert.AreEqual(Status.Failure, action_2.Status);
            Assert.AreEqual(Status.None, action_3.Status);
        }

        [TestMethod("Complex tree execution")]
        public void TestComplexTreeExec()
        {
            BehaviourTree tree = new BehaviourTree();

            var action_1 = tree.CreateLeafNode<ActionBTNode>("Nodo 1");
            action_1.Action = (st) => Status.Failure;
            var action_2 = tree.CreateLeafNode<ActionBTNode>("Nodo 2");
            action_2.Action = (st) => Status.Sucess;
            var action_3 = tree.CreateLeafNode<ActionBTNode>("Nodo 3");
            action_3.Action = (st) => Status.Failure;
            var composite_1 = tree.CreateComposite<SelectorNode>("Sel_1", false, action_1, action_2, action_3);

            var action_4 = tree.CreateLeafNode<ActionBTNode>("Nodo 4");
            action_4.Action = (st) => Status.Sucess;
            var action_5 = tree.CreateLeafNode<ActionBTNode>("Nodo 5");
            action_5.Action = (st) => Status.Sucess;
            var action_6 = tree.CreateLeafNode<ActionBTNode>("Nodo 6");
            action_6.Action = (st) => Status.Sucess;
            var composite_2 = tree.CreateComposite<SequencerNode>("Seq_2", false, action_4, action_5, action_6);

            var action_7 = tree.CreateLeafNode<ActionBTNode>("Nodo 7");
            action_7.Action = (st) => Status.Sucess;

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
            for (int i = 0; i < 5; i++) tree.Update();
            Assert.AreEqual(Status.Running, tree.Status);
            tree.Update();
            Assert.AreEqual(Status.Sucess, tree.Status);
        }
    }
}
