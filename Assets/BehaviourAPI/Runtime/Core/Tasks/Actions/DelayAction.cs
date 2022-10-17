using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public class DelayAction : ActionTask
    {
        public float DelayTime;
        public override void Start()
        {
            ExecutionTime = 0f;
        }

        public override void Update()
        {
            if (DelayTime > ExecutionTime)
            {
                Success();
            }
        }
    }
}