
namespace BehaviourAPI.UtilitySystems
{
    using Core;
    using Core.Actions;
    using System;
    using System.Collections.Generic;
    using Action = Core.Actions.Action;

    public class UtilityAction : UtilitySelectableNode, IActionHandler
    {
        #region ------------------------------------------ Properties ----------------------------------------
        public override string Description => "Utility node that executes an action and computes its utility with it child factor";
        public Action Action { get; set; }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        Factor _factor;

        public bool FinishSystemOnComplete = false;
        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public void SetFactor(Factor factor)
        {
            _factor = factor;
        }

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            if (children.Count > 0 && children[0] is Factor f)
                _factor = f;
            else
                throw new ArgumentException();
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        protected override float ComputeUtility()
        {
            _factor?.UpdateUtility();
            return _factor?.Utility ?? 0f;
        }

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
