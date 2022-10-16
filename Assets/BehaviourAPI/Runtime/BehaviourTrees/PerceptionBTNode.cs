using System.Collections;
using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using Core;

    /// <summary>
    /// A behaviour tree node that executes an <see cref="Perception"/> and translate the boolean returned value in a Status value.
    /// </summary>
    public class PerceptionBTNode : BTNode
    {
        public sealed override int MaxOutputConnections => 0;
        public Perception Perception;
        public override void Start()
        {
            Perception.Start();
        }

        public override void Initialize(Context context)
        {
            Perception?.Initialize(context);
        }

        public override Status UpdateStatus()
        {
            var perceptionValue = Perception.Check();
            return perceptionValue ? Status.Sucess : Status.Failure;
        }
    }
}