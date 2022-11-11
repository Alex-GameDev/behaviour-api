namespace BehaviourAPI.Testing
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
            var t2_3 = parent.CreateFinishStateTransition("t2_3", s2, s3, false, true);

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

            parent.Update(); // Exit S4 -> Enter S5 -> Exit subgraph (Failure) -> trigger exitgraphperception ->
                             // -> S2 (None) -> S3(running)
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
            bool bebido = false;
            StackFSM fsm = new StackFSM();
            Assert.AreEqual(1, fsm.Nodes.Count);
            var st1 = fsm.CreateState("andando", new FunctionalAction(() => Status.Running));
            var st2 = fsm.CreateState("luchando", new FunctionalAction(() => Status.Running));
            var st3 = fsm.CreateState("beber_poción", new FunctionalAction(() => Status.Running));
            var st4 = fsm.CreateState("tirar_poción", new FunctionalAction(() => Status.Running));
            var t1_2 = fsm.CreateTransition("enemigo", st1, st2, new ConditionPerception(() => detectaEnemigo));
            var t2_1 = fsm.CreateTransition("vencido", st2, st1, new ConditionPerception(() => !detectaEnemigo));
            var t1_3 = fsm.CreatePushTransition("caida", st1, st3, new ConditionPerception(() => herido));
            var t2_3 = fsm.CreatePushTransition("herido", st2, st3, new ConditionPerception(() => herido));
            var t3_4 = fsm.CreateTransition("curado", st3, st4, new ConditionPerception(() => bebido));
            var t3_r = fsm.CreatePopTransition<Transition>("recuperado_desde_tirar", st3, new ConditionPerception(() => !herido));
            var t4_r = fsm.CreatePopTransition<Transition>("recuperado_desde_beber", st4, new ConditionPerception(() => !herido));
            fsm.SetStartNode(st1);

            fsm.Start(); // start -> s1
            Assert.AreEqual(Status.Running, st1.Status);
            Assert.AreEqual(Status.None, st2.Status);
            Assert.AreEqual(Status.None, st3.Status);
            Assert.AreEqual(Status.None, st4.Status);

            herido = true;
            fsm.Update(); // s1 -> s3(push s1)
            Assert.AreEqual(Status.None, st1.Status);
            Assert.AreEqual(Status.None, st2.Status);
            Assert.AreEqual(Status.Running, st3.Status);
            Assert.AreEqual(Status.None, st4.Status);

            bebido = true;
            fsm.Update(); // s3 -> s4
            Assert.AreEqual(Status.None, st1.Status);
            Assert.AreEqual(Status.None, st2.Status);
            Assert.AreEqual(Status.None, st3.Status);
            Assert.AreEqual(Status.Running, st4.Status);
            bebido = false;
            herido = false;

            fsm.Update(); //s4 -> s1(pop s1)
            Assert.AreEqual(Status.Running, st1.Status);
            Assert.AreEqual(Status.None, st2.Status);
            Assert.AreEqual(Status.None, st3.Status);
            Assert.AreEqual(Status.None, st4.Status);

            detectaEnemigo = true;
            fsm.Update(); //s1 -> s2 
            Assert.AreEqual(Status.None, st1.Status);
            Assert.AreEqual(Status.Running, st2.Status);
            Assert.AreEqual(Status.None, st3.Status);
            Assert.AreEqual(Status.None, st4.Status);

            herido = true;
            fsm.Update(); //s2 -> s3 (push s2)
            Assert.AreEqual(Status.None, st1.Status);
            Assert.AreEqual(Status.None, st2.Status);
            Assert.AreEqual(Status.Running, st3.Status);
            Assert.AreEqual(Status.None, st4.Status);

            herido = false;
            fsm.Update(); //s3 -> s2 (pop s2)
            Assert.AreEqual(Status.None, st1.Status);
            Assert.AreEqual(Status.Running, st2.Status);
            Assert.AreEqual(Status.None, st3.Status);
            Assert.AreEqual(Status.None, st4.Status);
        }

        [TestMethod("Probability State")]
        public void Test_FSM_ProbabilityState()
        {
            var p1 = .5f;
            var p2 = .2f;
            var p3 = .3f;

            // Sum >= 1
            var sumProb = p1 + p2 + p3;
            FSM fsm = new FSM();

            var ps = fsm.CreateState<ProbabilisticState>("ps", new FunctionalAction(() => Status.Running));
            var s1 = fsm.CreateState("s1", new FunctionalAction(() => Status.Running));
            var s2 = fsm.CreateState("s2", new FunctionalAction(() => Status.Running));
            var s3 = fsm.CreateState("s3", new FunctionalAction(() => Status.Running));
            var tp1 = fsm.CreateProbabilisticTransition("tp1", ps, s1, p1);
            var tp2 = fsm.CreateProbabilisticTransition("tp2", ps, s2, p2);
            var tp3 = fsm.CreateProbabilisticTransition("tp3", ps, s3, p3);
            var t1p = fsm.CreateTransition("t1p", s1, ps, new ConditionPerception(() => true));
            var t2p = fsm.CreateTransition("t2p", s2, ps, new ConditionPerception(() => true));
            var t3p = fsm.CreateTransition("t3p", s3, ps, new ConditionPerception(() => true));

            fsm.Start();
            Assert.AreEqual(Status.Running, ps.Status);
            Assert.AreEqual(.5f, ps.GetProbability(tp1));            

            for(int i = 0; i < 1000; i++)
            {
                // ps <--> s1 (0.0 <= p < 0.5) 
                // ps <--> s2 (0.5 <= p < 0.7)
                // ps <--> s3 (0.7 <= p < 1.0)
                fsm.Update();
                Assert.IsTrue(ps.Prob < sumProb);

                State s;
                if (ps.Prob < p1) s = s1;
                else if (ps.Prob < p1 + p2) s = s2;
                else s = s3;

                Assert.AreEqual(Status.None, ps.Status);
                Assert.AreEqual(Status.Running, s.Status);
                fsm.Update();
                Assert.AreEqual(Status.Running, ps.Status);
                Assert.AreEqual(Status.None, s.Status);
            }
            
        }

        [TestMethod("Fire transitions")]
        public void Test_FSM_FireTransitions()
        {
            var fsm = new FSM();
            var s1 = fsm.CreateState("s1", new FunctionalAction(()=> Status.Running));
            var s2 = fsm.CreateState("s2", new FunctionalAction(() => Status.Running));
            var t1 = fsm.CreateTransition("t1", s1, s2);
            var t2 = fsm.CreateTransition("t2", s2, s1);

            var pushT1 = new PushPerception(t1);
            var pushT2 = new PushPerception(t2);

            // s1 <--> s2
            // Trigger transitions using Push perceptions
            fsm.Start();    // s1 (R) <--> s2(N)
            fsm.Update();   // s1 (R) <--> s2(N)

            Assert.AreEqual(Status.Running, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);

            pushT2.Fire(); // S2 is not current state -> s1 (R) <--> s2(N)
            Assert.AreEqual(Status.Running, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);

            pushT1.Fire(); // S1 is current state -> s1 (N) <--> s2(R)
            Assert.AreEqual(Status.None, s1.Status);
            Assert.AreEqual(Status.Running, s2.Status);

            pushT2.Fire(); // S2 is current state -> s1 (R) <--> s2(N)
            Assert.AreEqual(Status.Running, s1.Status);
            Assert.AreEqual(Status.None, s2.Status);
        }
    }
}