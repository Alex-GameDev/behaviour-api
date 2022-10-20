using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    [System.Serializable]
    public abstract class ActionTask : Task
    {
        public Status ExecutionStatus { get; private set; }

        public override void Start() => ExecutionStatus = Status.Running;
        public abstract void Update();
        public void Success() => ExecutionStatus = Status.Sucess;
        public void Failure() => ExecutionStatus = Status.Failure;
        public override void Reset() => ExecutionStatus = Status.None;
        public void SetStatus(Status status) => ExecutionStatus = status;
    }
}