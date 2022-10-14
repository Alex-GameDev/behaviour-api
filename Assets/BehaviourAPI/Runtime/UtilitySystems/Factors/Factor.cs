using System;
using System.Collections.Generic;
using BehaviourAPI.Runtime.Core;
using UnityEngine;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    using Core;

    public abstract class Factor : UtilityNode
    {
        public override Type ChildType => typeof(Factor);
        public override int MaxInputConnections => -1;
    }
}
