using BehaviourAPI.Core;
using System.Xml.Linq;

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

        public override void Initialize()
        {
            base.Initialize();
            if(OutputConnections.Count == 1)
            {
                _factor = GetChildNodes().First() as Factor;
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

        public override void Update()
        {
            Status = UpdateStatus();
        }

        protected abstract Status UpdateStatus();

        #endregion
    }
}
