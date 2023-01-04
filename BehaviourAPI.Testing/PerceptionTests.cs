namespace BehaviourAPI.Testing
{
    using BehaviourAPI.Core;
    using BehaviourAPI.Core.Actions;
    using BehaviourAPI.Core.Perceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StateMachines;
    [TestClass]
    public class PerceptionTests
    {
        [TestMethod]
        public void Test_Perception_ExecutionStatus()
        {
            var fsm = new FSM();
            var st1 = fsm.CreateActionState("st1", new FunctionalAction(() => Status.Success));
            var st2 = fsm.CreateActionState("st2", new FunctionalAction(() => Status.Failure));
            var st3 = fsm.CreateActionState("st3", new FunctionalAction(() => Status.Failure));
            var t1 = fsm.CreateTransition("t1", st1, st2, new ExecutionStatusPerception(st1, true, true));
            var t2 = fsm.CreateTransition("t2", st2, st3, new ExecutionStatusPerception(st2, false, true));
            var t3 = fsm.CreateTransition("t3", st3, st1, new ExecutionStatusPerception(st3, true, false));
            var t4 = fsm.CreateTransition("t4", st3, st2, new ExecutionStatusPerception(st3, false, true));

            fsm.Start();
            Assert.AreEqual(Status.Running, st1.Status);
            fsm.Update(); // st1 (success) -> t1 -> st2 (running)
            Assert.AreEqual(Status.Running, st2.Status);
            fsm.Update(); // s2 (failure) -> t2 -> st3 (running)
            Assert.AreEqual(Status.Running, st3.Status);
            fsm.Update(); // st1 (failure) -> t4 -> st2 (running)
            Assert.AreEqual(Status.Running, st2.Status);
        }

        [TestMethod]
        public void Test_StatusFlags()
        {
            Assert.IsTrue((StatusFlags.Success | StatusFlags.Failure & StatusFlags.Success) != 0);
            Assert.IsTrue((StatusFlags.Success | StatusFlags.Failure & StatusFlags.Failure) != 0);
            Assert.IsFalse((StatusFlags.Failure & StatusFlags.Success) != 0);
        }

        [TestMethod]
        public void Test_Perception_And()
        {
            bool a = false, b = false, c = false;
            var fsm = new FSM();
            var st1 = fsm.CreateActionState("st1", new FunctionalAction(() => Status.Success));
            var st2 = fsm.CreateActionState("st2", new FunctionalAction(() => Status.Failure));
            Perception p = new AndPerception(new ConditionPerception(() => a), new ConditionPerception(() => b), new ConditionPerception(() => c));
            var t = fsm.CreateTransition("t", st1, st2, p);
            PushPerception push = new PushPerception(fsm.CreateTransition("r", st2, st1));

            fsm.Start();
            fsm.Update(); // (false, false false) = false
            Assert.AreEqual(Status.None, st2.Status);
            a = true;
            fsm.Update(); // (true, false false) = false
            Assert.AreEqual(Status.None, st2.Status);
            b = true;
            fsm.Update(); // (true, true false) = false
            Assert.AreEqual(Status.None, st2.Status);
            c = true;
            fsm.Update(); // (true, true true) = true
            Assert.AreEqual(Status.Running, st2.Status);

            push.Fire();
            t.Perception = new AndPerception();
            fsm.Update(); // If and perception has no childs always return false
            Assert.AreEqual(Status.None, st2.Status);
        }

        [TestMethod]
        public void Test_Perception_Or()
        {
            bool a = false, b = false, c = false;
            var fsm = new FSM();
            var st1 = fsm.CreateActionState("st1", new FunctionalAction(() => Status.Success));
            var st2 = fsm.CreateActionState("st2", new FunctionalAction(() => Status.Failure));
            Perception p = new OrPerception(new ConditionPerception(() => a), new ConditionPerception(() => b), new ConditionPerception(() => c));
            var t = fsm.CreateTransition("t", st1, st2, p);
            PushPerception push = new PushPerception(fsm.CreateTransition("r", st2, st1));

            fsm.Start();
            fsm.Update(); // (false, false false) = false
            Assert.AreEqual(Status.None, st2.Status);

            a = true;
            fsm.Update(); // (true, false false) = true
            Assert.AreEqual(Status.Running, st2.Status);

            push.Fire();
            b = true;
            fsm.Update(); // (true, true false) = true
            Assert.AreEqual(Status.Running, st2.Status);

            push.Fire();
            c = true;
            fsm.Update(); // (true, true true) = true
            Assert.AreEqual(Status.Running, st2.Status);

            push.Fire();
            t.Perception = new OrPerception();
            fsm.Update(); // If and perception has no childs always return false
            Assert.AreEqual(Status.None, st2.Status);
        }
    }
}
