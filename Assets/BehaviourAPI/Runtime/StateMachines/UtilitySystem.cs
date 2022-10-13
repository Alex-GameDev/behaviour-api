using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviourAPI.Runtime.UtilitySystems
{

    using Core;
    public class UtilitySystem : BehaviourEngine
    {
        public override Type NodeType => typeof(UtilityNode);
        public override Type ConnectionType => typeof(UtilityConnection);

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