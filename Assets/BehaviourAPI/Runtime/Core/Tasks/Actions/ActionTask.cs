using System;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{

    /// <summary>
    /// Task that executes along time and return a Status value.
    /// </summary>
    public abstract class ActionTask : Task
    {
        /// <summary>
        /// The current status of the execution
        /// </summary>
        public Status ExecutionStatus { get; private set; }

        /// <summary>
        /// The elapsed time of the execution
        /// </summary>
        public float ExecutionTime
        {
            get => m_executionTime;
            protected set
            {
                m_executionTime = value;
                ExecutionTimeChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Event triggered every execution frame
        /// </summary>
        public Action<float> ExecutionTimeChanged { get; set; }

        float m_executionTime;

        public override void Start() => ExecutionStatus = Status.Running;

        public virtual void Update()
        {
            ExecutionTime += Time.deltaTime;
        }

        public void Success() => ExecutionStatus = Status.Sucess;

        public void Failure() => ExecutionStatus = Status.Failure;

        public override void Reset()
        {
            ExecutionTime = 0f;
            ExecutionStatus = Status.None;
        }

        public void SetStatus(Status status) => ExecutionStatus = status;
    }
}