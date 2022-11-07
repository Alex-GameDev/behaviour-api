namespace BehaviourAPI.Testing
{
    using Core;
    using BehaviourTrees;
    using StateMachines;
    using UtilitySystems;
    using Core.Actions;
    using BehaviourAPI.Core.Perceptions;

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

            fsm.Update(); //FSM (R) [None - Success - None] - Árbol (S) [Failure - Success]
            Assert.AreEqual(Status.Success, subBT.Status);
            Assert.AreEqual(Status.Success, bt.Status);
            Assert.AreEqual(Status.Success, action2.Status);

            fsm.Update(); //FSM (R) [None - None - Running] - Árbol (N)
            Assert.AreEqual(Status.None, subBT.Status);
            Assert.AreEqual(Status.Running, final.Status);
            Assert.AreEqual(Status.None, bt.Status);
            Assert.AreEqual(Status.None, action1.Status);
            Assert.AreEqual(Status.None, action2.Status);
        }
    }
}
