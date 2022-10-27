namespace BehaviourAPI.UtilitySystems
{
    public class VariableFactor : Factor
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override string Description => "Fusion factor that returns the value of a variable nromalized and clamped between 0 and 1.";
        public override int MaxOutputConnections => 0;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        public Func<float>? Variable;

        public float min, max;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        protected override float ComputeUtility()
        {
            Utility = Variable?.Invoke() ?? min;
            Utility = (Utility - min) / (max - min);
            return Math.Clamp(Utility, 0f, 1f);
        }

        #endregion
    }
}
