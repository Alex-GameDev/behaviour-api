using System;
using System.Collections.Generic;
using BehaviourAPI.Runtime.Core;
using UnityEngine;

namespace BehaviourAPI.Runtime.UtilitySystems
{

    /// <summary>
    /// Utility element that handle multiple <see cref="UtilityElement"/> itself and
    /// returns the maximum utility if its best candidate utility is higher than the threshold.
    /// </summary>
    public class UtilityAction : UtilityElement
    {
        public override Type ChildType => typeof(Factor);
        public override int MaxOutputConnections => 1;
        private Factor m_mainFactor;

        protected override float ComputeUtility()
        {
            m_mainFactor.UpdateUtility();
            Utility = m_mainFactor.Utility;
            return Utility;
        }

        public override void Update()
        {

        }
    }
}
