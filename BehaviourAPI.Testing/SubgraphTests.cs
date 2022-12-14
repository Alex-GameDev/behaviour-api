namespace BehaviourAPI.Testing
{
    using Core;
    using BehaviourTrees;
    using StateMachines;
    using UtilitySystems;
    using Core.Actions;
    using BehaviourAPI.Core.Perceptions;
    using BehaviourAPI.BehaviourTrees.Composites;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class SubgraphTests
    {

        [TestMethod("BT Sub FSM")]
        public void Test_BT_SubFSM()
        {
            BehaviourTree bt = new BehaviourTree();
            FSM fsm = new FSM();
            var action1 = bt.CreateLeafNode("action1", new FunctionalAction(() => Status.Success));
            var action2 = bt.CreateLeafNode("action2", new EnterSystemAction(fsm));
            var action3 = bt.CreateLeafNode("action3", new FunctionalAction(() => Status.Success));
            var seq = bt.CreateComposite<SequencerNode>("seq", false, action1, action2, action3);
            bt.SetRootNode(seq);

            var entry = fsm.CreateState("entry");
            var exit = fsm.CreateState("exit", new ExitSystemAction(fsm, Status.Success));
            var t = fsm.CreateTransition("t", entry, exit, new ConditionPerception(() => true));


            bt.Start();
            bt.Update(); //Árbol (R) [ None - Running - None] - FSM (R) [Running - None]
            Assert.AreEqual(Status.None, action1.Status);
            Assert.AreEqual(Status.Running, action2.Status);
            Assert.AreEqual(Status.Running, fsm.Status);
            Assert.AreEqual(Status.Running, entry.Status);

            bt.Update(); //Árbol (R) [ None - None - Running] - FSM (S) [None - None]
            Assert.AreEqual(Status.None, action2.Status);
            Assert.AreEqual(Status.Running, action3.Status);
            Assert.AreEqual(Status.None, fsm.Status);
            Assert.AreEqual(Status.None, exit.Status);

            bt.Update(); //Árbol (S) [ None - None - Success] - FSM (S) [None - None]
            Assert.AreEqual(Status.Success, action3.Status);
            Assert.AreEqual(Status.Success, bt.Status);
            Assert.AreEqual(Status.None, fsm.Status);
            Assert.AreEqual(Status.None, exit.Status);
        }

        [TestMethod("BT Sub FSM loop")]
        public void Test_BT_SubFSM_Loop()
        {
            float v1 = 1f, v2 = 0f;
            BehaviourTree bt = new BehaviourTree();
            FSM fsm = new FSM();
            UtilitySystem us = new UtilitySystem();

            var action1 = bt.CreateLeafNode("action1", new EnterSystemAction(us));
            var action2 = bt.CreateLeafNode("action2", new EnterSystemAction(fsm));
            var action3 = bt.CreateLeafNode("action3", new FunctionalAction(() => Status.Success));

            var seq = bt.CreateComposite<SequencerNode>("seq", false, action1, action2, action3);
            var loop = bt.CreateDecorator<IteratorNode>("loop", seq).SetIterations(-1);
            bt.SetRootNode(loop);

            var entry = fsm.CreateState("entry");
            var exit = fsm.CreateState("exit", new ExitSystemAction(fsm, Status.Success));
            var t = fsm.CreateTransition("t", entry, exit, new ConditionPerception(() => true));

            var f1 = us.CreateVariableFactor("f1", () => v1, 0, 1);
            var f2 = us.CreateVariableFactor("f2", () => v2, 0, 1);

            var u_action_1 = us.CreateUtilityAction("action1", f1, new FunctionalAction(() => Status.Success), finishOnComplete: true);
            var u_action_2 = us.CreateUtilityAction("action2", f2, new FunctionalAction(() => Status.Success), finishOnComplete: true);


            bt.Start();  //Árbol (R) [ Running - None - None] - FSM (R) [None - None] - US [None - None]
            Assert.AreEqual(Status.Running, bt.Status);
            Assert.AreEqual(Status.Running, us.Status);
            Assert.AreEqual(Status.None, fsm.Status);
            Assert.AreEqual(Status.Running, action1.Status);
            Assert.AreEqual(Status.None, action2.Status);
            Assert.AreEqual(Status.None, action3.Status);
            Assert.AreEqual(Status.None, u_action_1.Status);
            Assert.AreEqual(Status.None, u_action_2.Status);
            Assert.AreEqual(Status.None, entry.Status);
            Assert.AreEqual(Status.None, exit.Status);

            bt.Update(); //Árbol (R) [ None - Running - None] - FSM (R) [Running - None] - US [None - None]
            Assert.AreEqual(Status.Running, bt.Status);
            Assert.AreEqual(Status.None, us.Status);
            Assert.AreEqual(Status.Running, fsm.Status);
            Assert.AreEqual(Status.None, action1.Status);
            Assert.AreEqual(Status.Running, action2.Status);
            Assert.AreEqual(Status.None, action3.Status);
            Assert.AreEqual(Status.None, u_action_1.Status);
            Assert.AreEqual(Status.None, u_action_2.Status);
            Assert.AreEqual(Status.Running, entry.Status);
            Assert.AreEqual(Status.None, exit.Status);

            bt.Update(); //Árbol (R) [ None - None - Running] - FSM (S) [None - None] - US [N - N]
            Assert.AreEqual(Status.Running, bt.Status);
            Assert.AreEqual(Status.None, us.Status);
            Assert.AreEqual(Status.None, fsm.Status);
            Assert.AreEqual(Status.None, action1.Status);
            Assert.AreEqual(Status.None, action2.Status);
            Assert.AreEqual(Status.Running, action3.Status);
            Assert.AreEqual(Status.None, u_action_1.Status);
            Assert.AreEqual(Status.None, u_action_2.Status);
            Assert.AreEqual(Status.None, entry.Status);
            Assert.AreEqual(Status.None, exit.Status);

            bt.Update(); //Árbol (S) [ Running - None - None] - FSM (S) [None - None]
            Assert.AreEqual(Status.Running, bt.Status);
            Assert.AreEqual(Status.Running, us.Status);
            Assert.AreEqual(Status.None, fsm.Status);
            Assert.AreEqual(Status.Running, action1.Status);
            Assert.AreEqual(Status.None, action2.Status);
            Assert.AreEqual(Status.None, action3.Status);
            Assert.AreEqual(Status.None, u_action_1.Status);
            Assert.AreEqual(Status.None, u_action_2.Status);
            Assert.AreEqual(Status.None, entry.Status);
            Assert.AreEqual(Status.None, exit.Status);

            bt.Update(); //Árbol (R) [ None - Running - None] - FSM (R) [Running - None]
            Assert.AreEqual(Status.Running, bt.Status);
            Assert.AreEqual(Status.None, us.Status);
            Assert.AreEqual(Status.Running, fsm.Status);
            Assert.AreEqual(Status.None, action1.Status);
            Assert.AreEqual(Status.Running, action2.Status);
            Assert.AreEqual(Status.None, action3.Status);
            Assert.AreEqual(Status.None, u_action_1.Status);
            Assert.AreEqual(Status.None, u_action_2.Status);
            Assert.AreEqual(Status.Running, entry.Status);
            Assert.AreEqual(Status.None, exit.Status);
        }

        [TestMethod("FSM Sub BT")]
        public void Test_FSM_SubBT()
        {
            FSM fsm = new FSM();
            BehaviourTree bt = new BehaviourTree();
            var entry = fsm.CreateState("entry");
            var subBT = fsm.CreateState("subBT", new EnterSystemAction(bt));
            var final = fsm.CreateState("final");
            var t1 = fsm.CreateTransition("t1", entry, subBT, new ConditionPerception(() => true));
            var t2 = fsm.CreateFinishStateTransition<Transition>("t2", subBT, final, true, false);

            var action1 = bt.CreateLeafNode("action1", new FunctionalAction(() => Status.Failure));
            var action2 = bt.CreateLeafNode("action2", new FunctionalAction(() => Status.Success));
            var sel = bt.CreateComposite<SelectorNode>("seq", false, action1, action2);
            bt.SetRootNode(sel);


            fsm.Start();  //FSM (R) [Running - None - None] - Arbol(N)
            Assert.AreEqual(Status.Running, fsm.Status);
            Assert.AreEqual(Status.Running, entry.Status);
            Assert.AreEqual(Status.None, bt.Status);

            fsm.Update(); //FSM (R) [None - Running - None] - Árbol (R) [ Running - None - None] 
            Assert.AreEqual(Status.None, entry.Status);
            Assert.AreEqual(Status.Running, subBT.Status);
            Assert.AreEqual(Status.Running, bt.Status);
            Assert.AreEqual(Status.Running, action1.Status);

            fsm.Update(); //FSM (R) [None - Running - None] - Árbol (R) [Failure - Running]
            Assert.AreEqual(Status.Running, subBT.Status);
            Assert.AreEqual(Status.Running, bt.Status);
            Assert.AreEqual(Status.None, action1.Status);
            Assert.AreEqual(Status.Running, action2.Status);

            fsm.Update(); //FSM (R) [None - None - Running] - Árbol (N)
            Assert.AreEqual(Status.None, subBT.Status);
            Assert.AreEqual(Status.None, bt.Status);
            Assert.AreEqual(Status.None, action2.Status);
            Assert.AreEqual(Status.Running, final.Status);
        }

        [TestMethod("US Sub BT and FSM")]
        public void Test_US_SubBTFSM()
        {
            var v1 = 1f;
            var v2 = 0f;
            var v3 = 0f;
            UtilitySystem us = new UtilitySystem();
            var f1 = us.CreateVariableFactor("f1", () => v1, 0f, 1f);
            var f2 = us.CreateVariableFactor("f2", () => v2, 0f, 1f);
            var f3 = us.CreateVariableFactor("f3", () => v3, 0f, 1f);

            FSM fsm = new FSM();
            BehaviourTree bt = new BehaviourTree();

            var basic = us.CreateUtilityAction("base", f1, new FunctionalAction(()=> { v1 = 0f; v2 = 1f; }, () => Status.Running));
            var subfsm = us.CreateUtilityAction("fsm", f2, new EnterSystemAction(fsm));
            var subbt = us.CreateUtilityAction("bt", f3, new EnterSystemAction(bt));

            var entry = fsm.CreateState("entry");
            var action = fsm.CreateState("action", new FunctionalAction(() => { v2 = 0f; v3 = .5f; }, () => Status.Running));

            var t1 = fsm.CreateTransition("t1", entry, action, new ConditionPerception(() => true));

            var action1 = bt.CreateLeafNode("action1", new FunctionalAction(() => v1 = 1f, () => Status.Success));
            var action2 = bt.CreateLeafNode("action2", new FunctionalAction(() => v3 = 0f, () => Status.Success));
            var parallel = bt.CreateComposite<ParallelCompositeNode>("parallel", false, action1, action2);
            bt.SetRootNode(parallel);

            us.Start();

            us.Update(); // Us (basic-1, fsm-0, bt-0) -> basic.start() -> basic.update() -> v1 = 0, v2 = 1 
            Assert.AreEqual(Status.Running, basic.Status);
            Assert.AreEqual(1f, basic.Utility);
            Assert.AreEqual(0f, subfsm.Utility);
            Assert.AreEqual(0f, subbt.Utility);

            us.Update(); // US (basic-0, fsm-1, bt-0) -> basic.stop() -> fsm.start() -> fsm.Update() -> t1 -> action.Start() -> v2 = 0, v3 = 0
            Assert.AreEqual(Status.Running, subfsm.Status);
            Assert.AreEqual(Status.Running, fsm.Status);
            Assert.AreEqual(Status.Running, action.Status);
            Assert.AreEqual(0f, basic.Utility);
            Assert.AreEqual(1f, subfsm.Utility);
            Assert.AreEqual(0f, subbt.Utility);

            us.Update(); // US (basic-0, fsm-0, bt-1) -> fsm.stop() -> bt.start() -> bt.Update() -> Success -> v1 = 1, v3 = 0
            Assert.AreEqual(Status.None, fsm.Status);
            Assert.AreEqual(Status.Success, subbt.Status);
            Assert.AreEqual(Status.Success, bt.Status);
            Assert.AreEqual(Status.Success, action1.Status);
            Assert.AreEqual(Status.Success, action2.Status);
            Assert.AreEqual(0f, v3);
            Assert.AreEqual(1f, v1);
        }

        [TestMethod("Exception when execute graph twice")]
        public void Test_ExecuteGraphTwice()
        {
            BehaviourTree tree = new BehaviourTree();
            FSM fsm = new FSM();
            fsm.CreateState("State", new FunctionalAction(() => Status.Running));
            var a1 = tree.CreateLeafNode("sub1", new EnterSystemAction(fsm));
            var a2 = tree.CreateLeafNode("sub2", new EnterSystemAction(fsm));
            var parallel = tree.CreateComposite<ParallelCompositeNode>("parallel", false, a1, a2);
            tree.SetRootNode(parallel);
            Assert.ThrowsException<Exception>(() => tree.Start());           

        }
    }
}
