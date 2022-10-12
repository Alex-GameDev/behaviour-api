using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using Core;
    using UnityEngine.Events;

    /// <summary>
    /// A behaviour tree node that executes an <see cref="ActionTask"/>.
    /// </summary>
    public class ActionBTNode : BTNode
    {
        public sealed override int MaxOutputConnections => 0;
        public UnityEvent action;

        //public ActionTask Action { get; set; }
        public override void Start()
        {
            //Action.Start();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override Status UpdateStatus()
        {
            // Action.Update();
            // return Action.ExecutionStatus;
            return Status.Failure;
        }
    }
}