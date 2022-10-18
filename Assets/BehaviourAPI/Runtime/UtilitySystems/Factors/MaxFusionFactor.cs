using System;
using System.Collections.Generic;
using BehaviourAPI.Runtime.Core;
using UnityEngine;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    using System.Linq;

    public class MaxFusionFactor : FusionFactor
    {
        public override string Name => "Max Fusion Factor";
        public override string Description => "Fusion factor that returns the maximum value of its childs";
        protected override float ComputeUtility()
        {
            m_childFactors.ForEach(f => f.UpdateUtility());
            return m_childFactors.Max(f => f.Utility);
        }
    }
}
