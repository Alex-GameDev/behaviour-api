using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Testing
{
    using UtilitySystems;
    using Core;
    using BehaviourAPI.Core.Actions;

    [TestClass]
    public class UtilitySystemExecutionTests
    {
        [TestMethod]
        public void Test_UtilitySystem_VariableFactor()
        {
            var v1 = 0f;
            UtilitySystem us = new UtilitySystem();
            Factor f1 = us.CreateVariableFactor("v1", () => v1, 0f, 1f);
            UtilityAction action1 = us.CreateUtilityAction("Action", f1, new FunctionalAction(() =>
            {
                v1 += 1f;
                return Status.Running;
            }));

            us.Start();
            Assert.AreEqual(0f, f1.Utility);
            Assert.AreEqual(0f, v1);

            us.Update();
            Assert.AreEqual(0f, f1.Utility);
            Assert.AreEqual(1f, v1);

            us.Update();
            Assert.AreEqual(1f, f1.Utility);
            Assert.AreEqual(2f, v1);
        }

        [TestMethod]
        public void Test_UtilitySystem_BestElementSelection()
        {
            var v1 = 1f;
            var v2 = 0f;
            UtilitySystem us = new UtilitySystem();

            Factor f1 = us.CreateVariableFactor("v1", () => v1, 0f, 1f);
            Factor f2 = us.CreateVariableFactor("v2", () => v2, 0f, 1f);

            UtilityAction action1 = us.CreateUtilityAction("Action1", f1, new FunctionalAction(() =>
            {
                v1 -= .25f;
                v2 += .25f;
                return Status.Running;
            }));
            UtilityAction action2 = us.CreateUtilityAction("Action2", f2, new FunctionalAction(() =>
            {
                v1 += .25f;
                v2 -= .25f;
                return Status.Running;
            }));

            us.Start();

            Assert.AreEqual(0f, f1.Utility);
            Assert.AreEqual(0f, f2.Utility);
            Assert.AreEqual(0f, action1.Utility);
            Assert.AreEqual(0f, action2.Utility);

            us.Update();
            Assert.AreEqual(1f, f1.Utility);
            Assert.AreEqual(0f, f2.Utility);
            Assert.AreEqual(1f, action1.Utility);
            Assert.AreEqual(0f, action2.Utility);

            us.Update();
            Assert.AreEqual(.75f, action1.Utility);
            Assert.AreEqual(.25f, action2.Utility);

            us.Update();
            Assert.AreEqual(.5f, action1.Utility);
            Assert.AreEqual(.5f, action2.Utility);

            us.Update();
            Assert.AreEqual(.25f, action1.Utility);
            Assert.AreEqual(.75f, action2.Utility);

            us.Update();
            Assert.AreEqual(.5f, action1.Utility);
            Assert.AreEqual(.5f, action2.Utility);

            us.Update();
            Assert.AreEqual(.75f, action1.Utility);
            Assert.AreEqual(.25f, action2.Utility);
        }

        [TestMethod]
        public void Test_UtilitySystem_Buckets()
        {
            var v1 = 0f;
            var v2 = 0f;
            var v3 = 0f;
            var v4 = 0f;

            var actionId = 0;

            var us = new UtilitySystem(inertia: 1.0f, utilityThreshold: 0.2f);
            var f1 = us.CreateVariableFactor("f1", () => v1, 0f, 1f);
            var f2 = us.CreateVariableFactor("f2", () => v2, 0f, 1f);
            var f3 = us.CreateVariableFactor("f3", () => v3, 0f, 1f);
            var f4 = us.CreateVariableFactor("f4", () => v4, 0f, 1f);
            var action_1 = us.CreateUtilityAction("Action_1", f1, new FunctionalAction(() => actionId = 1, () => Status.Running));
            var action_2 = us.CreateUtilityAction("Action_2", f2, new FunctionalAction(() => actionId = 2, () => Status.Running), root: false);
            var action_3 = us.CreateUtilityAction("Action_3", f3, new FunctionalAction(() => actionId = 3, () => Status.Running), root: false);
            var bucket = us.CreateUtilityBucket("Bucket", true, 0.1f, 1.0f, 0.5f, action_2, action_3);
            var action_4 = us.CreateUtilityAction("Action_4", f4, new FunctionalAction(() => actionId = 4, () => Status.Running));

            us.Start();
            Assert.AreEqual(Status.None, us.Status);
            Assert.AreEqual(Status.None, action_1.Status);
            Assert.AreEqual(Status.None, bucket.Status);
            Assert.AreEqual(Status.None, action_2.Status);

            v1 = 0f; v2 = .15f; v3 = .1f; v4 = .15f;
            us.Update(); // utility < UtilityThreshold -> Default action
            Assert.AreEqual(0f, action_1.Utility);
            Assert.AreEqual(.15f, action_2.Utility);
            Assert.AreEqual(.1f, action_3.Utility);
            Assert.AreEqual(.15f, action_4.Utility);
            Assert.AreEqual(.15f, bucket.Utility);
            Assert.AreEqual(1, actionId);

            v1 = .5f; v2 = .2f; v3 = .2f; v4 = .3f;
            us.Update(); // Action1 > Bucket > Action 3
            Assert.AreEqual(.5f, action_1.Utility);
            Assert.AreEqual(.2f, action_2.Utility);
            Assert.AreEqual(.2f, action_3.Utility);
            Assert.AreEqual(.3f, action_4.Utility);
            Assert.AreEqual(.2f, bucket.Utility);
            Assert.AreEqual(1, actionId);

            v1 = .1f; v2 = .6f; v3 = .2f; v4 = .8f;
            us.Update(); // Bucket is locked (v3 > bucket.threshold)
            Assert.AreEqual(.1f, action_1.Utility);
            Assert.AreEqual(.6f, action_2.Utility);
            Assert.AreEqual(.2f, action_3.Utility);
            Assert.AreEqual(.3f, action_4.Utility); // This utility is not recalculated
            Assert.AreEqual(.6f, bucket.Utility);
            Assert.AreEqual(2, actionId);

            v1 = .1f; v2 = .4f; v3 = .2f; v4 = .8f;
            us.Update(); // Bucket is not locked (v3 < bucket.threshold)
            Assert.AreEqual(.1f, action_1.Utility);
            Assert.AreEqual(.4f, action_2.Utility);
            Assert.AreEqual(.2f, action_3.Utility);
            Assert.AreEqual(.8f, action_4.Utility);
            Assert.AreEqual(.4f, bucket.Utility);
            Assert.AreEqual(4, actionId);

            // TODO: Problema: Si la utilidad del bucket es superior a su umbral pero menor al umbral
            // del us, la utilidad devuelta sería 1, cuando deberia descartarse
        }

        [TestMethod]
        public void Test_UtilitySystem_FunctionFactor()
        {
            var v1 = 0f;
            UtilitySystem us = new UtilitySystem();
            
            Factor f1 = us.CreateVariableFactor("v1", () => v1, 0f, 1f);
            FunctionFactor ff = us.CreateFunctionFactor("ff", f1, new CustomFunction((x) => x * x));
            UtilityAction action = us.CreateUtilityAction("Action1", ff, new FunctionalAction(() =>
            {
                v1 += .25f;
                return Status.Running;
            }));

            us.Start();            
            us.Update();
            Assert.AreEqual(0f, f1.Utility);
            Assert.AreEqual(0f, ff.Utility);
            Assert.AreEqual(0f, action.Utility);

            Assert.AreEqual(.25f, v1);
            us.Update();            
            Assert.AreEqual(.25f, f1.Utility);
            Assert.AreEqual(.0625f, ff.Utility);
            Assert.AreEqual(.0625f, action.Utility);

            Assert.AreEqual(.5f, v1);
            us.Update();
            Assert.AreEqual(.5f, f1.Utility);
            Assert.AreEqual(.25f, ff.Utility);
            Assert.AreEqual(.25f, action.Utility);

        }

        [TestMethod]
        public void Test_UtilitySystem_FusionFactor()
        {
            var v1 = 2;
            var v2 = 5;
            var v3 = 10;
            UtilitySystem us = new UtilitySystem();

            Factor f1 = us.CreateVariableFactor("v1", () => v1, 0, 10);
            Factor f2 = us.CreateVariableFactor("v2", () => v2, 0, 10);
            Factor f3 = us.CreateVariableFactor("v3", () => v3, 0, 10);
            FusionFactor ff = us.CreateFusionFactor<MaxFusionFactor>("ff", f1, f2, f3);
            UtilityAction action = us.CreateUtilityAction("Action1", ff, new FunctionalAction(() =>
            {
                v1 += 1;
                v3 -= 3;
                return Status.Running;
            }));

            us.Start();
            us.Update();
            Assert.AreEqual(1f, action.Utility);
            us.Update();
            Assert.AreEqual(.7f, action.Utility);
            us.Update();
            Assert.AreEqual(.5f, action.Utility);
            us.Update();
            Assert.AreEqual(.5f, action.Utility);
            us.Update();
            Assert.AreEqual(.6f, action.Utility);
        }

        [TestMethod("Connection repeated")]
        public void Test_US_ConnectionRepeated()
        {
            UtilitySystem us = new UtilitySystem();
            var a = us.CreateVariableFactor("A", () => 0f, 0f, 1f);

            Assert.ThrowsException<ArgumentException>(() => us.CreateFusionFactor<MinFusionFactor>("F", a, a));
            
        }

        [TestMethod("Connection loop")]
        public void Test_US_ConnectionLoop()
        {
            UtilitySystem us = new UtilitySystem();
            var a = (FunctionFactor)us.CreateNode(typeof(FunctionFactor));
            var b = (FunctionFactor)us.CreateNode(typeof(FunctionFactor));
            var c = (FunctionFactor)us.CreateNode(typeof(FunctionFactor));

            us.CreateConnection(us.ConnectionType, a, b);
            us.CreateConnection(us.ConnectionType, b, c);
            us.CreateConnection(us.ConnectionType, a, c);

            Assert.ThrowsException<ArgumentException>(() => us.CreateConnection(us.ConnectionType, a, a));
            Assert.ThrowsException<ArgumentException>(() => us.CreateConnection(us.ConnectionType, b, a));
            Assert.ThrowsException<ArgumentException>(() => us.CreateConnection(us.ConnectionType, c, a));

        }
    }
 }
