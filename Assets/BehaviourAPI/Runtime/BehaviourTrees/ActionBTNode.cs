using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using Core;

    /// <summary>
    /// A behaviour tree node that executes an <see cref="ActionTask"/>.
    /// </summary>
    public class ActionBTNode : BTNode, ITaskHandler<ActionTask>
    {
        public override string Name => "Action Node";
        public override string Description => "Behaviour Tree Node that executes an Action";
        public override int MaxOutputConnections => 0;
        public ActionTask Task { get => m_action; set => m_action = value; }

        [SerializeField] ActionTask m_action;

        public override void Initialize(Context context)
        {
            base.Initialize(context);
            Task?.Initialize(context);
        }

        public override void Reset()
        {
            base.Reset();
            Task?.Reset();
        }

        public override void Start()
        {
            base.Start();
            Task.Start();
        }

        public override Status UpdateStatus()
        {
            Task.Update();
            return Task.ExecutionStatus;
        }
    }
}