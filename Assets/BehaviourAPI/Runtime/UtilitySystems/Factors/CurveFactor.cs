using System;
using System.Collections.Generic;
using BehaviourAPI.Runtime.Core;
using UnityEngine;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    using Core;

    /// <summary>
    /// Factor that modifies its child value with a function.
    /// </summary>
    public class CurveFactor : Factor
    {
        public override int MaxOutputConnections => 1;
        public AnimationCurve function;
        Factor m_childFactor;

        protected override float ComputeUtility()
        {
            m_childFactor.UpdateUtility();
            return function.Evaluate(m_childFactor.Utility);
        }
    }
}
