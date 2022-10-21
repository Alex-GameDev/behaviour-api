using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public class TimerPerception : Perception
    {
        public float DelayTime { get; set; }
        float m_time;
        public override void Start()
        {
            m_time = 0f;
        }
        public override bool Check()
        {
            m_time += Time.deltaTime;
            return DelayTime > m_time;
        }

        public override void Reset()
        {
            m_time = 0f;
        }
    }
}