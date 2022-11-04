namespace BehaviourAPI.Testing
{
    using Core;
    using BehaviourTrees;
    using Core.Actions;
    using BehaviourAPI.Core.Perceptions;
    using System;

    [TestClass]
    public class BehaviourTreeExecutionTests
    {
        [TestMethod("Inverter Decorator")]
        public void Test_BT_Inverter()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateLeafNode<PerceptionBTNode>("Nodo 1");
            action_1.Perception = new ConditionPerception(() => false);
            var inv = tree.CreateDecorator<InverterNode>("inv", action_1);
            tree.SetStartNode(inv);

            tree.Start();
            Assert.AreEqual(Status.Running, inv.Status);

            tree.Update();
            Assert.AreEqual(Status.Sucess, inv.Status);
            Assert.AreEqual(Status.Failure, action_1.Status);

            action_1.Perception = new ConditionPerception(() => true);
            tree.Update();
            Assert.AreEqual(Status.Failure, inv.Status);
            Assert.AreEqual(Status.Sucess, action_1.Status);
        }

        [TestMethod("Succeder Decorator")]
        public void Test_BT_Succeder()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateLeafNode<PerceptionBTNode>("Nodo 1");
            action_1.Perception = new ConditionPerception(() => false);
            var suc = tree.CreateDecorator<SuccederNode>("inv", action_1);
            tree.SetStartNode(suc);

            tree.Start();
            Assert.AreEqual(Status.Running, suc.Status);

            tree.Update();
            Assert.AreEqual(Status.Sucess, suc.Status);
            Assert.AreEqual(Status.Failure, action_1.Status);

            action_1.Perception = new ConditionPerception(() => true);
            tree.Update();
            Assert.AreEqual(Status.Sucess, suc.Status);
            Assert.AreEqual(Status.Sucess, action_1.Status);
        }

        [TestMethod("Iterator Decorator")]
        public void Test_BT_Iterator()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateLeafNode<PerceptionBTNode>("Nodo 1");
            action_1.Perception = new ConditionPerception(() => true);
            var iter = tree.CreateDecorator<IteratorNode>("inv", action_1).SetIterations(3);
            tree.SetStartNode(iter);

            tree.Start();
            Assert.AreEqual(Status.Running, iter.Status);

            tree.Update(); // Action ends with success -> iters = 1 -> iter restart action 
            Assert.AreEqual(Status.Running, iter.Status);
            Assert.AreEqual(Status.Running, action_1.Status);

            tree.Update(); // Action ends with success -> iters = 2 -> iter restart action 
            Assert.AreEqual(Status.Running, iter.Status);
            Assert.AreEqual(Status.Running, action_1.Status);

            tree.Update(); // Action ends with success -> iters = 3 -> keep the value 
            Assert.AreEqual(Status.Sucess, iter.Status);
            Assert.AreEqual(Status.Sucess, action_1.Status);
        }

        [TestMethod("LoopUntil Decorator")]
        public void Test_BT_LoopUntil()
        {
            int i = 0;
            int j = 0;

            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateLeafNode<PerceptionBTNode>("Nodo 1");
            var action_2 = tree.CreateLeafNode<PerceptionBTNode>("Nodo 2");
            action_1.Perception = new ConditionPerception(() => i == 3);
            action_2.Perception = new ConditionPerception(() => j == 3);
            var loopUntil1 = tree.CreateDecorator<LoopUntilNode>("l1", action_1).SetTargetStatus(Status.Sucess);
            var loopUntil2 = tree.CreateDecorator<LoopUntilNode>("l2", action_2).SetTargetStatus(Status.Sucess).SetMaxIterations(2);
            var seq = tree.CreateComposite<SequencerNode>("seq", false, loopUntil1, loopUntil2);
            tree.SetStartNode(seq);

            tree.Start();

            i++;
            tree.Update(); // i = 1 -> Action1 end with failure -> iters = 1 -> loopUntil1 restart action1
            Assert.AreEqual(Status.Running, loopUntil1.Status);
            Assert.AreEqual(Status.Running, action_1.Status);

            i++;
            tree.Update(); // i = 2 -> Action1 end with failure -> iters = 2 -> loopUntil1 restart action1
            Assert.AreEqual(Status.Running, loopUntil1.Status);
            Assert.AreEqual(Status.Running, action_1.Status);

            i++;
            tree.Update(); // i = 3 -> Action1 end with success -> keep the value
            Assert.AreEqual(Status.Sucess, loopUntil1.Status);
            Assert.AreEqual(Status.Sucess, action_1.Status);

            j++;
            tree.Update(); // j = 1 -> Action1 end with failure -> iters = 1 -> loopUntil1 restart action1
            Assert.AreEqual(Status.Running, loopUntil2.Status);
            Assert.AreEqual(Status.Running, action_2.Status);

            j++;
            tree.Update(); // j = 1 -> Action1 end with failure -> iters = 2 -> keep the value
            Assert.AreEqual(Status.Failure, loopUntil2.Status);
            Assert.AreEqual(Status.Failure, action_2.Status);
        }

        [TestMethod("Condiotion Decorator")]
        public void Test_BT_ConditionDecorator()
        {
            int i = 0;

            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateLeafNode<PerceptionBTNode>("Nodo 1");
            action_1.Perception = new ConditionPerception(() => true);
            var cond = tree.CreateDecorator<ConditionDecoratorNode>("inv", action_1);
            cond.Perception = new ConditionPerception(() => i > 2 && i < 4);
            tree.SetStartNode(cond);

            tree.Start();

            i++;
            tree.Update(); // i = 1 -> cond == false
            Assert.AreEqual(Status.Running, cond.Status);
            Assert.AreEqual(Status.None, action_1.Status);

            i++;
            tree.Update(); // i = 2 -> cond == false
            Assert.AreEqual(Status.Running, cond.Status);
            Assert.AreEqual(Status.None, action_1.Status);

            i++;
            tree.Update(); // i = 3 -> cond == true -> child.Start();
            Assert.AreEqual(Status.Sucess, cond.Status);
            Assert.AreEqual(Status.Sucess, action_1.Status);

            i++;
            tree.Update(); // i = 4 -> cond == false -> child.Stop();
            Assert.AreEqual(Status.Running, cond.Status);
            Assert.AreEqual(Status.None, action_1.Status);

        }


        [TestMethod("Sequencer Execution Sucess")]
        public void Test_BT_Sequencer_Success()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateLeafNode<ActionBTNode>("Nodo 1").SetAction(new FunctionalAction(() => Status.Sucess));
            var action_2 = tree.CreateLeafNode<ActionBTNode>("Nodo 2").SetAction(new FunctionalAction(() => Status.Sucess));
            var action_3 = tree.CreateLeafNode<ActionBTNode>("Nodo 3").SetAction(new FunctionalAction(() => Status.Sucess));
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
        public void Test_BT_Sequencer_Failure()
        {
            BehaviourTree tree = new BehaviourTree();
            var action_1 = tree.CreateLeafNode<ActionBTNode>("Nodo 1").SetAction(new FunctionalAction(() => Status.Sucess));
            var action_2 = tree.CreateLeafNode<ActionBTNode>("Nodo 2").SetAction(new FunctionalAction(() => Status.Failure));
            var action_3 = tree.CreateLeafNode<ActionBTNode>("Nodo 3").SetAction(new FunctionalAction(() => Status.Sucess));
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
        public void Test_BT_MultilevelTree()
        {
            BehaviourTree tree = new BehaviourTree();

            var action_1 = tree.CreateLeafNode<ActionBTNode>("Nodo 1");
            action_1.Action = new FunctionalAction(() => Status.Failure);
            var action_2 = tree.CreateLeafNode<ActionBTNode>("Nodo 2");
            action_2.Action = new FunctionalAction(() => Status.Sucess);
            var action_3 = tree.CreateLeafNode<ActionBTNode>("Nodo 3");
            action_3.Action = new FunctionalAction(() => Status.Failure);
            var composite_1 = tree.CreateComposite<SelectorNode>("Sel_1", false, action_1, action_2, action_3);

            var action_4 = tree.CreateLeafNode<ActionBTNode>("Nodo 4");
            action_4.Action = new FunctionalAction(() => Status.Sucess);
            var action_5 = tree.CreateLeafNode<ActionBTNode>("Nodo 5");
            action_5.Action = new FunctionalAction(() => Status.Sucess);
            var action_6 = tree.CreateLeafNode<ActionBTNode>("Nodo 6");
            action_6.Action = new FunctionalAction(() => Status.Sucess);
            var composite_2 = tree.CreateComposite<SequencerNode>("Seq_2", false, action_4, action_5, action_6);

            var action_7 = tree.CreateLeafNode<ActionBTNode>("Nodo 7");
            action_7.Action = new FunctionalAction(() => Status.Sucess);

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
