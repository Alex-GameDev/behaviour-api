using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
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
    public class FSMExecutionTests
    {
        [TestMethod("FSM two state execution")]
        public void FSMDouble()
        {
            FSM fsm = new FSM();
            var s1 = fsm.CreateState<ActionState>("st1").SetAction(new FunctionalAction(() => Status.Running));
            var s2 = fsm.CreateState<ActionState>("st2").SetAction(new FunctionalAction(() => Status.Running));
            Transition t = fsm.CreateTransition<Transition>(s1, s2, new ConditionalPerception(() => true));

            fsm.Start();
            Assert.AreEqual(Status.Running, fsm.Status);
            Assert.AreEqual(Status.Running, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);

            fsm.Update();
            Assert.AreEqual(Status.Running, fsm.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.Running, s2.Status);

            fsm.Stop();
            Assert.AreEqual(Status.None, fsm.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
        }

        [TestMethod("FSM three state execution")]
        public void FSMTriple()
        {
            FSM fsm = new FSM();
            var s1 = fsm.CreateState<ActionState>("st1").SetAction(new FunctionalAction(() => Status.Running));
            var s2 = fsm.CreateState<ActionState>("st2").SetAction(new FunctionalAction(() => Status.Running));
            var s3 = fsm.CreateState<ActionState>("st3").SetAction(new FunctionalAction(() => Status.Running));

            Transition t1_2 = fsm.CreateTransition<Transition>(s1, s2, new ConditionalPerception(() => false));
            Transition t1_3 = fsm.CreateTransition<Transition>(s1, s3, new ConditionalPerception(() => true));

            Transition t2_1 = fsm.CreateTransition<Transition>(s2, s1, new ConditionalPerception(() => false));
            Transition t2_3 = fsm.CreateTransition<Transition>(s2, s3, new ConditionalPerception(() => false));

            Transition t3_1 = fsm.CreateTransition<Transition>(s3, s2, new ConditionalPerception(() => true));
            Transition t3_2 = fsm.CreateTransition<Transition>(s3, s1, new ConditionalPerception(() => true));

            fsm.Start();
            Assert.AreEqual(Status.Running, fsm.Status);
            Assert.AreEqual(Status.Running, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);

            fsm.Update();
            Assert.AreEqual(Status.Running, fsm.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
            Assert.AreEqual(Status.Running, s3.Status);

            fsm.Update();
            Assert.AreEqual(Status.Running, fsm.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.Running, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);

            fsm.Update();
            Assert.AreEqual(Status.Running, fsm.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.Running, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);

            fsm.Stop();
            Assert.AreEqual(Status.None, fsm.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);
        }

        [TestMethod("FSM subgraph state execution")]
        public void FSMSubgraph()
        {
            // S1 ===> S2(subgraph) ===(Check S2.Status == Failure)==> S3
            //              ||
            //              \/
            //              S4 ===> S5 (Exit with Failure)

            FSM child = new FSM();
            FSM parent = new FSM();

            var s1 = parent.CreateState<ActionState>("st1").SetAction(new FunctionalAction(() => Status.Running));
            var s2 = parent.CreateState<SubgraphState>("st2").SetSubgraph(child);
            var s3 = parent.CreateState<ActionState>("st3").SetAction(new FunctionalAction(() => Status.Running));
            var t1_2 = parent.CreateTransition<Transition>(s1, s2, new ConditionalPerception(() => true));
            var t2_3 = parent.CreateFinishStateTransition<Transition>(s2, s3, false, true);

            var s4 = child.CreateState<ActionState>("st4").SetAction(new FunctionalAction(() => Status.Running));
            var s5 = child.CreateState<ExitState>("st5").SetReturnedStatus(Status.Failure);
            var t4_5 = child.CreateTransition<Transition>(s4, s5, new ConditionalPerception(() => true));

            parent.Start();
            Assert.AreEqual(Status.Running, parent.Status);
            Assert.AreEqual(Status.None, child.Status);
            Assert.AreEqual(Status.Running, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);
            Assert.AreEqual(Status.None, s4.Status);
            Assert.AreEqual(Status.None, s5.Status);

            parent.Update();
            Assert.AreEqual(Status.Running, parent.Status);
            Assert.AreEqual(Status.Running, child.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.Running, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);
            Assert.AreEqual(Status.Running, s4.Status);
            Assert.AreEqual(Status.None, s5.Status);

            parent.Update();
            Assert.AreEqual(Status.Running, parent.Status);
            Assert.AreEqual(Status.Failure, child.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.Failure, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);
            Assert.AreEqual(Status.None, s4.Status);
            Assert.AreEqual(Status.None, s5.Status);

            parent.Update();
            Assert.AreEqual(Status.Running, parent.Status);
            Assert.AreEqual(Status.None, child.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
            Assert.AreEqual(Status.Running, s3.Status);
            Assert.AreEqual(Status.None, s4.Status);
            Assert.AreEqual(Status.None, s5.Status);

            parent.Stop();
            Assert.AreEqual(Status.None, parent.Status);
            Assert.AreEqual(Status.None, child.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);
            Assert.AreEqual(Status.None, s4.Status);
            Assert.AreEqual(Status.None, s5.Status);

        }

        [TestMethod("FSM mealy transition execution")]
        public void FSMMealyTransition()
        {
            // S1 -> i = -1 -> s2 -> i = 0 -> s3 -> i = 1 -> s2 -> i = 0 -> s1

            var i = 0;
            FSM fsm = new FSM();
            var s1 = fsm.CreateState<ActionState>("st1").SetAction(new FunctionalAction(() => Status.Sucess));
            var s2 = fsm.CreateState<ActionState>("st2").SetAction(new FunctionalAction(() => Status.Sucess));
            var s3 = fsm.CreateState<ActionState>("st3").SetAction(new FunctionalAction(() => Status.Sucess));
            var t1_2 = fsm.CreateTransition<MealyTransition>(s1, s2, new ConditionalPerception(() => true)).SetOnPerformAction(() => i--);
            var t2_3 = fsm.CreateTransition<MealyTransition>(s2, s3, new ConditionalPerception(() => i < 0)).SetOnPerformAction(() => i++); 
            var t3_2 = fsm.CreateTransition<MealyTransition>(s3, s2, new ConditionalPerception(() => true)).SetOnPerformAction(() => i++); 
            var t2_1 = fsm.CreateTransition<MealyTransition>(s2, s1, new ConditionalPerception(() => i > 0)).SetOnPerformAction(() => i--);

            fsm.Start();
            Assert.AreEqual(0, i);
            Assert.AreEqual(Status.Running, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);

            fsm.Update();
            Assert.AreEqual(-1, i);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.Running, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);

            fsm.Update();
            Assert.AreEqual(0, i);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
            Assert.AreEqual(Status.Running, s3.Status);

            fsm.Update();
            Assert.AreEqual(1, i);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.Running, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);


            fsm.Update();
            Assert.AreEqual(0, i);
            Assert.AreEqual(Status.Running, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);
        }
    }
}