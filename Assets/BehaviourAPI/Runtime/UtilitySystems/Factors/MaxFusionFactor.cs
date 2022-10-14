using System;
using System.Collections.Generic;
using BehaviourAPI.Runtime.Core;
using UnityEngine;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    using System.Linq;

    public class MaxFactor : FusionFactor
    {
        protected override float ComputeUtility()
        {
            m_childFactors.ForEach(f => f.UpdateUtility());
            return m_childFactors.Max(f => f.Utility);
        }
    }
}
