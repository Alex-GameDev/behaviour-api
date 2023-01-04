using BehaviourAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.UtilitySystems.UtilityElements
{
    public abstract class UtilityExecutableNode : UtilitySelectableNode
    {
        #region ------------------------------------------ Properties ----------------------------------------

        public override Type ChildType => typeof(Factor);
        public override int MaxOutputConnections => 1;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        Factor _factor;

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

        #endregion
    }
}
