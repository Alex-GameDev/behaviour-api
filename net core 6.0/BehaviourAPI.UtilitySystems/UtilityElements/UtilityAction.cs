using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using System.Xml.Linq;

namespace BehaviourAPI.UtilitySystems
{
    using Core.Actions;
    public class UtilityAction : UtilitySelectableNode, IActionHandler
    {
        #region ------------------------------------------ Properties ----------------------------------------
        
        public override Type ChildType => typeof(Factor);
        public override int MaxOutputConnections => 1;

        public Action? Action { get; set; }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        Factor? _factor;

        public bool FinishSystemOnComplete = false;
        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public void SetFactor(Factor factor)
        {
            _factor = factor;
        }

        public override void Initialize()
        {
            base.Initialize();
            if(Children.Count == 1)
            {
                _factor = GetFirstChild() as Factor;
            }
            else
            {
                throw new Exception();
            }
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
            Action?.Start();
        }

        public override void Update()
        {
            Status = Action?.Update() ?? Status.Error;
        }

        public override void Stop()
        {
            Action?.Stop();
        }

        public override bool FinishExecutionWhenActionFinishes() => FinishSystemOnComplete;

        #endregion
    }
}
