using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using System;
    using Core;

    /// <summary>
    /// The base node in the <see cref="BehaviourTree"/>.
    /// </summary>
    public abstract class BTNode : Node, IStatusHandler
    {
        public override int MaxInputConnections => 1;

        public override Type ChildType => typeof(BTNode);

        public Status Status { get; protected set; }
        public Action<Status> OnValueChanged { get; set; }

        BTNode parentNode; //TODO: Set parent node in OnConnection

        public BTNode()
        {

        }

        public abstract void Start();

        public void Update()
        {
            Status = UpdateStatus();
            OnValueChanged?.Invoke(Status);
        }

        public abstract Status UpdateStatus();
    }
}