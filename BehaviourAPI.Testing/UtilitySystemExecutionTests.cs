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
            UtilityAction action1 = us.CreateUtilityElement<UtilityAction>("Action1", f1).SetAction(new FunctionalAction(() =>
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

            UtilityAction action1 = us.CreateUtilityElement<UtilityAction>("Action1", f1).SetAction(new FunctionalAction(() =>
            {
                v1 -= .25f;
                v2 += .25f;
                return Status.Running;
            }));
            UtilityAction action2 = us.CreateUtilityElement<UtilityAction>("Action2", f2).SetAction(new FunctionalAction(() =>
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

        }

        [TestMethod]
        public void Test_UtilitySystem_FunctionFactor()
        {
            var v1 = 0f;
            UtilitySystem us = new UtilitySystem();
            
            Factor f1 = us.CreateVariableFactor("v1", () => v1, 0f, 1f);
            FunctionFactor ff = us.CreateFunctionFactor("ff", f1, new CustomFunction((x) => x * x));
            UtilityAction action = us.CreateUtilityElement<UtilityAction>("Action1", ff).SetAction(new FunctionalAction(() =>
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
            UtilityAction action = us.CreateUtilityElement<UtilityAction>("Action1", ff).SetAction(new FunctionalAction(() =>
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
    }
 }
