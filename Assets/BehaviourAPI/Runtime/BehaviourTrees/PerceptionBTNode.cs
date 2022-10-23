using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using Core;


    /// <summary>
    /// A behaviour tree node that executes an <see cref="Perception"/> and translate the boolean returned value in a Status value.
    /// </summary>
    public class PerceptionBTNode : BTNode, ITaskHandler<Perception>
    {
        public override string Name => "Perception Node";
        public override string Description => "Behaviour Tree Node that returns Success or failure depending on a Perception";
        public override int MaxOutputConnections => 0;
        public Perception Task { get => m_perception; set => m_perception = value; }

        [HideInInspector][SerializeField] Perception m_perception;

        public override void Initialize(Context context)
        {
            base.Initialize(context);
            Task?.Initialize(context);
        }
        public override void Start()
        {
            base.Start();
            Task.Start();
        }

        public override Status UpdateStatus()
        {
            var perceptionValue = Task.Check();
            return perceptionValue ? Status.Sucess : Status.Failure;
        }
    }
}