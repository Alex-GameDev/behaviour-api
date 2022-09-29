using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public class TimerPerception : Perception
    {
        public float DelayTime { get; set; }
        public override void Start()
        {
            ExecutionTime = 0f;
        }
        public override bool Check()
        {
            return DelayTime > ExecutionTime;
        }

        public override void Reset()
        {
            ExecutionTime = 0f;
        }
    }
}