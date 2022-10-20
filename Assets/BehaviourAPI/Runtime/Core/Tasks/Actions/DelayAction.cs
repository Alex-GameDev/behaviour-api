using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public class DelayAction : ActionTask
    {
        public float DelayTime;
        public override void Start()
        {
            base.Start();
            ExecutionTime = 0f;
        }

        public override void Update()
        {
            ExecutionTime += Time.deltaTime;
            if (ExecutionTime > DelayTime)
            {
                Success();
            }
        }
    }
}