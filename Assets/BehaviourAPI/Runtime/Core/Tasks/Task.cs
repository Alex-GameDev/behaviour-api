using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class Task
    {
        public float ExecutionTime { get; protected set; }
        public Context ExecutionContext { get; protected set; }

        public abstract void Start();
        public abstract void Reset();

        void foo()
        {
        }
    }
}