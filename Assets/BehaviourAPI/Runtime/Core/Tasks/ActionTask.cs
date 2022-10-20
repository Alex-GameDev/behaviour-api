using System;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    [System.Serializable]
    public abstract class ActionTask : Task
    {
        public Status ExecutionStatus { get; private set; }

        public float ExecutionTime
        {
            get => m_executionTime;
            protected set
            {
                m_executionTime = value;
                ExecutionTimeChanged?.Invoke(value);
            }
        }

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
            Debug.Log("Action reset");
            ExecutionTime = 0f;
            ExecutionStatus = Status.None;
        }

        public void SetStatus(Status status) => ExecutionStatus = status;
    }
}