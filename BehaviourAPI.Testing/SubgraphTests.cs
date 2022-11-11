namespace BehaviourAPI.Testing
{
    using Core;
    using BehaviourTrees;
    using StateMachines;
    using UtilitySystems;
    using Core.Actions;
    using BehaviourAPI.Core.Perceptions;
    using BehaviourAPI.BehaviourTrees.Composites;

    [TestClass]
    public class SubgraphTests
    {
        [TestMethod("BT Sub FSM")]
        public void Test_BT_SubFSM()
        {
            BehaviourTree bt = new BehaviourTree();
            FSM fsm = new FSM();
            var action1 = bt.CreateActionBTNode("action1", new FunctionalAction(() => Status.Success));
            var action2 = bt.CreateActionBTNode("action2", new EnterGraphAction(fsm));
            var action3 = bt.CreateActionBTNode("action3", new FunctionalAction(() => Status.Success));
            var seq = bt.CreateComposite<SequencerNode>("seq", false, action1, action2, action3);
            bt.SetStartNode(seq);

            var entry = fsm.CreateState("entry");
            var exit = fsm.CreateState("exit", new ExitGraphAction(fsm, Status.Success));
            var t = fsm.CreateTransition("t", entry, exit, new ConditionPerception(() => true));


            bt.Start();
            bt.Update(); //Árbol (R) [ Success - Running - None] - FSM (R) [Running - None]
            Assert.AreEqual(Status.Success, action1.Status);
            Assert.AreEqual(Status.Running, action2.Status);
            Assert.AreEqual(Status.Running, fsm.Status);
            Assert.AreEqual(Status.Running, entry.Status);

            bt.Update(); //Árbol (R) [ Success - Success - Running] - FSM (S) [None - Running]
            Assert.AreEqual(Status.Success, action2.Status);
            Assert.AreEqual(Status.Running, action3.Status);
            Assert.AreEqual(Status.Success, fsm.Status);
            Assert.AreEqual(Status.Running, exit.Status);

            bt.Update(); //Árbol (S) [ Success - Success - Success] - FSM (S) [None - Running]
            Assert.AreEqual(Status.Success, action3.Status);
            Assert.AreEqual(Status.Success, bt.Status);
            Assert.AreEqual(Status.Success, fsm.Status);
            Assert.AreEqual(Status.Running, exit.Status);
        }

        [TestMethod("FSM Sub BT")]
        public void Test_FSM_SubBT()
        {
            FSM fsm = new FSM();
            BehaviourTree bt = new BehaviourTree();
            var entry = fsm.CreateState("entry");
            var subBT = fsm.CreateState("subBT", new EnterGraphAction(bt));
            var final = fsm.CreateState("final");
            var t1 = fsm.CreateTransition("t1", entry, subBT, new ConditionPerception(() => true));
            var t2 = fsm.CreateFinishStateTransition<Transition>("t2", subBT, final, true, false);

            var action1 = bt.CreateActionBTNode("action1", new FunctionalAction(() => Status.Failure));
            var action2 = bt.CreateActionBTNode("action2", new FunctionalAction(() => Status.Success));
            var sel = bt.CreateComposite<SelectorNode>("seq", false, action1, action2);
            bt.SetStartNode(sel);


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
            Assert.AreEqual(Status.Failure, action1.Status);
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
            var subfsm = us.CreateUtilityAction("fsm", f2, new EnterGraphAction(fsm));
            var subbt = us.CreateUtilityAction("bt", f3, new EnterGraphAction(bt));

            var entry = fsm.CreateState("entry");
            var action = fsm.CreateState("action", new FunctionalAction(() => { v2 = 0f; v3 = .5f; }, () => Status.Running));

            var t1 = fsm.CreateTransition("t1", entry, action, new ConditionPerception(() => true));

            var action1 = bt.CreateActionBTNode("action1", new FunctionalAction(() => v1 = 1f, () => Status.Success));
            var action2 = bt.CreateActionBTNode("action2", new FunctionalAction(() => v3 = 0f, () => Status.Success));
            var parallel = bt.CreateComposite<ParallelCompositeNode>("parallel", false, action1, action2);
            bt.SetStartNode(parallel);

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
            var a1 = tree.CreateActionBTNode("sub1", new EnterGraphAction(fsm));
            var a2 = tree.CreateActionBTNode("sub2", new EnterGraphAction(fsm));
            var parallel = tree.CreateComposite<ParallelCompositeNode>("parallel", false, a1, a2);
            tree.SetStartNode(parallel);
            Assert.ThrowsException<Exception>(() => tree.Start());           

        }
    }
}
