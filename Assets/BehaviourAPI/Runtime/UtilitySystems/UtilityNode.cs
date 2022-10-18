using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    using System;
    using Core;
    public abstract class UtilityNode : Node, IUtilityHandler
    {
        public override int MaxInputConnections => -1;
        public float Utility { get; protected set; }
        public Action<float> OnValueChanged { get; set; }

        /// <summary>
        /// Updates the current value of <see cref="Utility"/>
        /// </summary>
        public void UpdateUtility()
        {
            Utility = ComputeUtility();
            OnValueChanged?.Invoke(Utility);
        }

        protected abstract float ComputeUtility();
    }
}