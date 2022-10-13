using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    using Core;
    public abstract class UtilityNode : Node, IUtilityHandler
    {
        public override int MaxInputConnections => -1;
        public float Utility { get; protected set; }
    }
}