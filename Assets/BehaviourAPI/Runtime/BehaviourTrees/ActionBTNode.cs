using System.Collections.Generic;
using UnityEngine.Events;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using Core;


    /// <summary>
    /// A behaviour tree node that executes an <see cref="ActionTask"/>.
    /// </summary>
    public class ActionBTNode : BTNode
    {
        public override string Name => "Action Node";
        public override string Description => "Behaviour Tree Node that executes an Action";
        public override int MaxOutputConnections => 0;
        public ActionTask Action;

        public override void Initialize(Context context)
        {
            Action?.Initialize(context);
        }

        public override void Start()
        {
            Action.Start();
        }

        public override Status UpdateStatus()
        {
            Action.Update();
            return Action.ExecutionStatus;
        }
    }
}