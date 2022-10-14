using System;
using System.Collections.Generic;
using BehaviourAPI.Runtime.Core;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourAPI.Runtime.UtilitySystems
{

    public class VariableFactor : Factor
    {
        public override int MaxOutputConnections => 0;
        public Func<float> Variable;

        protected override float ComputeUtility()
        {
            Utility = Variable.Invoke();
            return Utility;
        }
    }
}
