using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    [System.Serializable]
    public abstract class Task
    {
        public float ExecutionTime { get; protected set; }
        public Context ExecutionContext { get; protected set; }

        public virtual void Initialize(Context context)
        {
            ExecutionContext = context;
        }

        public abstract void Start();
        public abstract void Reset();
    }
}