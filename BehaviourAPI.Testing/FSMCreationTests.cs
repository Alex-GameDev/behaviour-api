using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Testing
{
    [TestClass]
    public class FSMCreationTests
    {
        [TestMethod("FSM two state creation")]
        public void FSMDoubleCreation()
        {
            FSM fsm = new FSM();
            var s1 = fsm.CreateState<ActionState>("st1");
            var s2 = fsm.CreateState<ActionState>("st2");
            Transition t = fsm.CreateTransition<Transition>(s1, s2, new ConditionalPerception(() => true));

            Assert.AreEqual(2, fsm.Nodes.Count);
            Assert.AreEqual(1, fsm.Connections.Count);
            Assert.AreEqual(s1, t.SourceNode);
            Assert.AreEqual(s2, t.TargetNode);
            Assert.AreEqual(fsm.StartNode, s1);
        }

        [TestMethod("FSM multiple state creation")]
        public void FSMMultipleCreation()
        {
            FSM fsm = new FSM();
            var s1 = fsm.CreateState<ActionState>("st1");
            var s2 = fsm.CreateState<ActionState>("st2");
            var s3 = fsm.CreateState<ActionState>("st1");
            var s4 = fsm.CreateState<ActionState>("st2");
            var s5 = fsm.CreateState<ActionState>("st1");
            var s6 = fsm.CreateState<ActionState>("st2");

            Transition t12 = fsm.CreateTransition<Transition>(s1, s2, new ConditionalPerception(() => true));
            Transition t13 = fsm.CreateTransition<Transition>(s1, s3, new ConditionalPerception(() => true));
            Transition t14 = fsm.CreateTransition<Transition>(s1, s4, new ConditionalPerception(() => true));
            Transition t23 = fsm.CreateTransition<Transition>(s2, s3, new ConditionalPerception(() => true));
            Transition t24_1 = fsm.CreateTransition<Transition>(s2, s4, new ConditionalPerception(() => true));
            Transition t24_2 = fsm.CreateTransition<Transition>(s2, s4, new ConditionalPerception(() => true));
            Transition t4_5 = fsm.CreateTransition<Transition>(s4, s5, new ConditionalPerception(() => true));
            Transition t3_6 = fsm.CreateTransition<Transition>(s3, s6, new ConditionalPerception(() => true));

            fsm.SetStartNode(s3);
            Assert.AreEqual(6, fsm.Nodes.Count);
            Assert.AreEqual(8, fsm.Connections.Count);
            Assert.AreEqual(fsm.StartNode, s3);
            Assert.AreEqual(3, s2.OutputConnections.Count);
            Assert.AreEqual(2, s3.InputConnections.Count);
            Assert.AreEqual(true, s2.IsParentOf(s4));
        }
    }
}
