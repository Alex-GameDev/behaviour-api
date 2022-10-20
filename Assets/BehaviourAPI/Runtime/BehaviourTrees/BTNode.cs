using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using System;
    using Core;
    using UnityEngine;

    /// <summary>
    /// The base node in the <see cref="BehaviourTree"/>.
    /// </summary>
    public abstract class BTNode : Node, IStatusHandler
    {
        public override int MaxInputConnections => 1;

        public override Type ChildType => typeof(BTNode);

        public Status Status
        {
            get => m_status;
            protected set
            {
                if (m_status != value)
                {
                    //Debug.Log($"Set status from {m_status} to {value}");
                    m_status = value;
                    OnValueChanged?.Invoke(m_status);
                }
            }
        }
        public Action<Status> OnValueChanged { get; set; }

        BTNode m_parentNode;
        Status m_status;

        public BTNode() { }

        public override void Initialize(Context context)
        {
            base.Initialize(context);
            Status = Status.None;
        }

        public override void Reset()
        {
            Status = Status.None;
        }

        public virtual void Start()
        {
            Status = Status.Running;
        }

        public void Update()
        {
            Status = UpdateStatus();
            OnValueChanged?.Invoke(Status);
        }

        public abstract Status UpdateStatus();
    }
}