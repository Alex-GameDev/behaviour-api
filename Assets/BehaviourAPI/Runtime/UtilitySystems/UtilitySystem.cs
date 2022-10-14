using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    using Core;

    /// <summary>
    /// Behaviour graph that choose between diferent <see cref="UtilityElement"/> items and executes.
    /// </summary>
    public class UtilitySystem : BehaviourEngine
    {
        public float Inertia = .3f;
        public override Type NodeType => typeof(UtilityNode);
        public override Type ConnectionType => typeof(UtilityConnection);

        List<UtilityElement> m_utilityElements;
        UtilityElement currentBestAction;

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }

}