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
    public class UtilityAction : UtilityElement, IActionAsignable
    {
        public override string Name => "Utility Action";
        public override string Description => "Utility element that executes a given action.";
        public override Type ChildType => typeof(Factor);
        public override int MaxOutputConnections => 1;
        public ActionTask Action { get => m_action; set => m_action = value; }
        [HideInInspector] ActionTask m_action;
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
