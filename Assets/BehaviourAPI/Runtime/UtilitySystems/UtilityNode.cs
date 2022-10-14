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

        /// <summary>
        /// Updates the current value of <see cref="Utility"/>
        /// </summary>
        public void UpdateUtility()
        {
            Utility = ComputeUtility();
        }

        protected abstract float ComputeUtility();
    }
}