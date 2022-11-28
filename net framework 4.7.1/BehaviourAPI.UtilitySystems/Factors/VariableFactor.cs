using System;

namespace BehaviourAPI.UtilitySystems
{
    using Core;
    public class VariableFactor : Factor
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override int MaxOutputConnections => 0;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        public Func<float> Variable;

        public float min, max;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        protected override float ComputeUtility()
        {
            Utility = Variable?.Invoke() ?? min;
            Utility = (Utility - min) / (max - min);
            return MathUtilities.Clamp01(Utility);
        }

        #endregion
    }
}
