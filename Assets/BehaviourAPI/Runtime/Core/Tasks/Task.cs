using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    /// <summary>
    /// Class that gives behaviour to the graphs.
    /// </summary>
    public abstract class Task : ScriptableObject
    {
        /// <summary>
        /// The execution context (agent)
        /// </summary>
        public Context ExecutionContext { get; protected set; }

        /// <summary>
        /// Prepare the task for the execution
        /// </summary>
        /// <param name="context">The execution context</param>
        public virtual void Initialize(Context context)
        {
            ExecutionContext = context;
        }

        /// <summary>
        /// Call at the beginning of the execution
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Call at the end of the execution
        /// </summary>
        public abstract void Reset();
    }
}