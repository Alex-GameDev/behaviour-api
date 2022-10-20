using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class Task : ScriptableObject
    {
        public Context ExecutionContext { get; protected set; }

        public virtual void Initialize(Context context)
        {
            ExecutionContext = context;
        }

        public abstract void Start();
        public abstract void Reset();
    }
}