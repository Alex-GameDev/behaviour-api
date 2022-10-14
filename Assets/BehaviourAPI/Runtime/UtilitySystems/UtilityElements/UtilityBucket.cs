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
    public class UtilityBucket : UtilityElement
    {
        public override Type ChildType => typeof(UtilityElement);
        public override int MaxOutputConnections => -1;
        [Range(0f, 1f)] public float UtilityThreshold = .3f;
        [Range(0f, 1f)] public float inertia = .3f;
        List<UtilityElement> m_utilityCandidates;

    }
}
