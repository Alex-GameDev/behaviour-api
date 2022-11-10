﻿namespace BehaviourAPI.Testing
{
    using Core;
    using Core.Actions;
    using Core.Perceptions;
    using StateMachines;

    [TestClass]
    public class FSMExecutionTests
    {
        [TestMethod]
        public void Test_FSM_Transition()
        {
            FSM fsm = new FSM();
            var s1 = fsm.CreateState("st1", new FunctionalAction(() => Status.Running));
            var s2 = fsm.CreateState("st2", new FunctionalAction(() => Status.Running));
            Transition t = fsm.CreateTransition<Transition>("t", s1, s2, new ConditionPerception(() => true));

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

        [TestMethod]
        public void Test_FSM_MultipleTransitions()
        {
            FSM fsm = new FSM();
            var s1 = fsm.CreateState("st1", new FunctionalAction(() => Status.Running));
            var s2 = fsm.CreateState("st2", new FunctionalAction(() => Status.Running));
            var s3 = fsm.CreateState("st3", new FunctionalAction(() => Status.Running));

            Transition t1_2 = fsm.CreateTransition<Transition>("t12", s1, s2, new ConditionPerception(() => false));
            Transition t1_3 = fsm.CreateTransition<Transition>("t13", s1, s3, new ConditionPerception(() => true));

            Transition t2_1 = fsm.CreateTransition<Transition>("t21", s2, s1, new ConditionPerception(() => false));
            Transition t2_3 = fsm.CreateTransition<Transition>("t23", s2, s3, new ConditionPerception(() => false));

            Transition t3_1 = fsm.CreateTransition<Transition>("t31", s3, s2, new ConditionPerception(() => true));
            Transition t3_2 = fsm.CreateTransition<Transition>("t32", s3, s1, new ConditionPerception(() => true));

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

        [TestMethod]
        public void Test_FSM_SubFSM()
        {
            // S1 ===> S2(subgraph) ===(Check S2.Status == Failure)==> S3
            //              ||
            //              \/
            //              S4 ===> S5 (Exit with Failure)

            FSM child = new FSM();
            FSM parent = new FSM();

            var s1 = parent.CreateState("st1", new FunctionalAction(() => Status.Running));
            var s2 = parent.CreateState("st2", new EnterGraphAction(child));
            var s3 = parent.CreateState("st3", new FunctionalAction(() => Status.Running));
            var t1_2 = parent.CreateTransition<Transition>("t12", s1, s2, new ConditionPerception(() => true));
            var t2_3 = parent.CreateFinishStateTransition("t2_3",s2, s3, false, true);

            var s4 = child.CreateState("st4", new FunctionalAction(() => Status.Running));
            var s5 = child.CreateState("st5", new ExitGraphAction(child, Status.Failure));
            var t4_5 = child.CreateTransition<Transition>("t45", s4, s5, new ConditionPerception(() => true));

            parent.Start(); // Enter S1
            Assert.AreEqual(Status.Running, parent.Status);
            Assert.AreEqual(Status.None, child.Status);
            Assert.AreEqual(Status.Running, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);
            Assert.AreEqual(Status.None, s4.Status);
            Assert.AreEqual(Status.None, s5.Status);

            parent.Update(); // Exit S1 -> Enter S2 -> Enter Subgraph -> Enter S4
            Assert.AreEqual(Status.Running, parent.Status);
            Assert.AreEqual(Status.Running, child.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.Running, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);
            Assert.AreEqual(Status.Running, s4.Status);
            Assert.AreEqual(Status.None, s5.Status);

            parent.Update(); // Exit S4 -> Enter S5 -> Exit subgraph (failure) -> S2 (failure)
            Assert.AreEqual(Status.Running, parent.Status);
            Assert.AreEqual(Status.Failure, child.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.Failure, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);
            Assert.AreEqual(Status.None, s4.Status);
            Assert.AreEqual(Status.Running, s5.Status); // ?

            parent.Update(); // Perception checked -> Exit S2 -> Enter S4
            Assert.AreEqual(Status.Running, parent.Status);
            Assert.AreEqual(Status.None, child.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
            Assert.AreEqual(Status.Running, s3.Status);
            Assert.AreEqual(Status.None, s4.Status);
            Assert.AreEqual(Status.None, s5.Status);

            parent.Stop(); // Exit graph
            Assert.AreEqual(Status.None, parent.Status);
            Assert.AreEqual(Status.None, child.Status);
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
            Assert.AreEqual(Status.None, s3.Status);
            Assert.AreEqual(Status.None, s4.Status);
            Assert.AreEqual(Status.None, s5.Status);

        }

        [TestMethod]
        public void Test_FSM_MealyTransition()
        {
            // S1 -> i = -1 -> s2 -> i = 0 -> s3 -> i = 1 -> s2 -> i = 0 -> s1

            var i = 0;
            FSM fsm = new FSM();
            var s1 = fsm.CreateState("st1", new FunctionalAction(() => Status.Success));
            var s2 = fsm.CreateState("st2", new FunctionalAction(() => Status.Success));
            var s3 = fsm.CreateState("st3", new FunctionalAction(() => Status.Success));
            var t1_2 = fsm.CreateTransition("t12", s1, s2, new ConditionPerception(() => true), new FunctionalAction(() => i--));
            var t2_3 = fsm.CreateTransition("t23", s2, s3, new ConditionPerception(() => i < 0), new FunctionalAction(() => i++)); 
            var t3_2 = fsm.CreateTransition("t32", s3, s2, new ConditionPerception(() => true), new FunctionalAction(() => i++)); 
            var t2_1 = fsm.CreateTransition("t21", s2, s1, new ConditionPerception(() => i > 0), new FunctionalAction(() => i--));

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

        [TestMethod("Stack FSM")]
        public void Test_FSM_StackFSM()
        {
            bool detectaEnemigo = false;
            bool herido = false;
            StackFSM fsm = new StackFSM();
            Assert.AreEqual(1, fsm.Nodes.Count);
            var st1 = fsm.CreateState("andando", new FunctionalAction(() => Status.Running));
            var st2 = fsm.CreateState("luchando", new FunctionalAction(() => Status.Running));
            var st3 = fsm.CreateState("beber_poción", new FunctionalAction(() => Status.Running));
            var t1_2 = fsm.CreateTransition("enemigo", st1, st2, new ConditionPerception(() => detectaEnemigo));
            var t2_1 = fsm.CreateTransition("vencido", st2, st1, new ConditionPerception(() => !detectaEnemigo));
            var t1_3 = fsm.CreateTransition("caida", st1, st3, new ConditionPerception(() => herido));
            var t2_3 = fsm.CreateTransition("herido", st2, st3, new ConditionPerception(() => herido));
            var t3_r = fsm.CreateComebackTransition<Transition>("recuperado", st3, new ConditionPerception(() => !herido));
            fsm.SetStartNode(st1);

            fsm.Start();
            Assert.AreEqual(Status.Running, st1.Status);
            Assert.AreEqual(Status.None, st2.Status);
            Assert.AreEqual(Status.None, st3.Status);

            herido = true;
            fsm.Update(); // 
            Assert.AreEqual(Status.None, st1.Status);
            Assert.AreEqual(Status.None, st2.Status);
            Assert.AreEqual(Status.Running, st3.Status);

            herido = false;
            fsm.Update();
            Assert.AreEqual(Status.Running, st1.Status);
            Assert.AreEqual(Status.None, st2.Status);
            Assert.AreEqual(Status.None, st3.Status);

            detectaEnemigo = true;
            fsm.Update();
            Assert.AreEqual(Status.None, st1.Status);
            Assert.AreEqual(Status.Running, st2.Status);
            Assert.AreEqual(Status.None, st3.Status);

            herido = true;
            fsm.Update();
            Assert.AreEqual(Status.None, st1.Status);
            Assert.AreEqual(Status.None, st2.Status);
            Assert.AreEqual(Status.Running, st3.Status);

            herido = false;
            fsm.Update();
            Assert.AreEqual(Status.None, st1.Status);
            Assert.AreEqual(Status.Running, st2.Status);
            Assert.AreEqual(Status.None, st3.Status);

        }
    }
}