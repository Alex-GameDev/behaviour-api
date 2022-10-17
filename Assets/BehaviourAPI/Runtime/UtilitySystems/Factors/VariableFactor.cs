using System;
using System.Collections.Generic;
using BehaviourAPI.Runtime.Core;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    public class VariableFactor : Factor
    {
        public override string Name => "Min Fusion Factor";
        public override string Description => "Fusion factor that returns the value of a variable clamped between 0 and 1.";
        public override int MaxOutputConnections => 0;
        public Func<float> Variable;

        protected override float ComputeUtility()
        {
            Utility = Variable.Invoke();
            return Utility;
        }
    }
}
