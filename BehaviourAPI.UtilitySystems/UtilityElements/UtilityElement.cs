using BehaviourAPI.Core;

namespace BehaviourAPI.UtilitySystems
{
    public abstract class UtilityElement : UtilitySelectableNode
    {
        #region ------------------------------------------ Properties ----------------------------------------
        
        public override Type ChildType => typeof(Factor);
        public override int MaxOutputConnections => 1;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        Factor? _factor;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public void SetFactor(Factor factor)
        {
            _factor = factor;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        protected override float ComputeUtility()
        {
            _factor?.UpdateUtility();
            return _factor?.Utility ?? 0f;
        }

        public override void Update()
        {
            Status = UpdateStatus();
        }

        protected abstract Status UpdateStatus();

        #endregion
    }
}
