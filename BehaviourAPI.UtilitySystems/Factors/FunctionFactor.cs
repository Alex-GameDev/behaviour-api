namespace BehaviourAPI.UtilitySystems
{
    using BehaviourAPI.UtilitySystems;
    using Core;

    /// <summary>
    /// Factor that modifies its child value with a function.
    /// </summary>  
    public class FunctionFactor : Factor
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override int MaxOutputConnections => 1;
        public override string Description => "Factor that modifies the child value with a function.";

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        public UtilityFunction? function;

        Factor? m_childFactor;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public FunctionFactor SetChild(Factor factor)
        {
            m_childFactor = factor;
            return this;
        }          

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        protected override float ComputeUtility()
        {
            m_childFactor?.UpdateUtility();
            return function?.Evaluate(m_childFactor?.Utility ?? 0f) ?? 0f;
        }

        #endregion
    }
}
