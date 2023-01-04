
namespace BehaviourAPI.UtilitySystems
{
    using BehaviourAPI.UtilitySystems.UtilityElements;
    using Core;
    using Core.Actions;
    using System;
    using System.Collections.Generic;
    using Action = Core.Actions.Action;

    public class UtilityAction : UtilityExecutableNode
    {
        #region ------------------------------------------ Properties ----------------------------------------
        
        public Action Action { get; set; }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        public bool FinishSystemOnComplete = false;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            Status = Status.Running;
            Action?.Start();
        }

        public override void Update()
        {
            Status = Action.Update();
        }

        public override void Stop()
        {
            Status = Status.None;
            Action?.Stop();
        }

        public override bool FinishExecutionWhenActionFinishes() => FinishSystemOnComplete;

        #endregion
    }
}
