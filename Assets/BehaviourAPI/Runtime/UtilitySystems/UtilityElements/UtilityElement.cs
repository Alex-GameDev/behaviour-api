using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace BehaviourAPI.Runtime.UtilitySystems
{
    using Core;

    public abstract class UtilityElement : UtilityNode, IStatusHandler
    {
        public Status Status { get; protected set; }

        public abstract void Update();
    }
}
