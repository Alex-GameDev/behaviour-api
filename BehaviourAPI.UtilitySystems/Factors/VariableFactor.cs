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

        public Variable<float> min, max;

        public VariableFactor()
        {
            Variable = null;
            min = 0.0f;
            max = 1.0f;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        protected override float ComputeUtility()
        {
            Utility = Variable?.Invoke() ?? min.Value;
            Utility = (Utility - min.Value) / (max.Value - min.Value);
            return MathUtilities.Clamp01(Utility);
        }

        #endregion
    }
}
