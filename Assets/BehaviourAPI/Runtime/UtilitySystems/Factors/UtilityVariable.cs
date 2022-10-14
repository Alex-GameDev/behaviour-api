using System;
using System.Collections.Generic;
using BehaviourAPI.Runtime.Core;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourAPI.Runtime.UtilitySystems
{

    public class UtilityVariable : Factor
    {
        public override int MaxOutputConnections => 0;

        public Func<float> Variable;
        public void ComputeUtility()
        {
            var utility = Variable?.Invoke();
        }

    }
}
