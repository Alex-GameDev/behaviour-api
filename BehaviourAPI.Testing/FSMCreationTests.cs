namespace BehaviourAPI.Testing
{
    using Core.Exceptions;
    using Core.Perceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StateMachines;
    [TestClass]
    public class FSMCreationTests
    {
        [TestMethod]
        public void Test_EmptyGraph()
        {
            var fsm = new FSM();
            Assert.ThrowsException<EmptyGraphException>(fsm.Start);
        }

        [TestMethod("FSM two state creation")]
        public void Test_FSM_Simple_Creation()
        {
            FSM fsm = new FSM();
            var s1 = fsm.CreateState("st1");
            var s2 = fsm.CreateState("st2");
            Transition t = fsm.CreateTransition<Transition>("t_s1_s2", s1, s2, new ConditionPerception(() => true));

            Assert.AreEqual(3, fsm.Nodes.Count);
            Assert.AreEqual(s1, t.GetFirstParent());
            Assert.AreEqual(s2, t.GetFirstChild());
            Assert.AreEqual(fsm.StartNode, s1);
        }

        [TestMethod("FSM multiple state creation")]
        public void Test_FSM_Complex_Creation()
        {
            FSM fsm = new FSM();
            var s1 = fsm.CreateState("st1");
            var s2 = fsm.CreateState("st2");
            var s3 = fsm.CreateState("st3");
            var s4 = fsm.CreateState("st4");
            var s5 = fsm.CreateState("st5");
            var s6 = fsm.CreateState("st6");

            Transition t12 = fsm.CreateTransition<Transition>("t12", s1, s2, new ConditionPerception(() => true));
            Transition t13 = fsm.CreateTransition<Transition>("t13", s1, s3, new ConditionPerception(() => true));
            Transition t14 = fsm.CreateTransition<Transition>("t14", s1, s4, new ConditionPerception(() => true));
            Transition t23 = fsm.CreateTransition<Transition>("t23", s2, s3, new ConditionPerception(() => true));
            Transition t24_1 = fsm.CreateTransition<Transition>("t241", s2, s4, new ConditionPerception(() => true));
            Transition t24_2 = fsm.CreateTransition<Transition>("t242", s2, s4, new ConditionPerception(() => true));
            Transition t4_5 = fsm.CreateTransition<Transition>("t45", s4, s5, new ConditionPerception(() => true));
            Transition t3_6 = fsm.CreateTransition<Transition>("t36", s3, s6, new ConditionPerception(() => true));

            fsm.SetStartNode(s3);
            Assert.AreEqual(14, fsm.Nodes.Count);
            Assert.AreEqual(fsm.StartNode, s3);
            Assert.AreEqual(3, s2.Children.Count);
            Assert.AreEqual(2, s3.Parents.Count);
            Assert.AreEqual(true, s2.IsParentOf(t23));
            Assert.AreEqual(true, t3_6.IsParentOf(s6));
        }

        [TestMethod]
        public void Test_FSM_LoopTransition()
        {
            FSM fsm = new FSM();
            var s1 = fsm.CreateState("st1");

            Transition t12 = fsm.CreateTransition<Transition>("t12", s1, s1, new ConditionPerception(() => true));

            Assert.AreEqual(2, fsm.Nodes.Count);
        }
    }
}
