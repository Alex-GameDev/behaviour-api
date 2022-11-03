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

        public override void Initialize()
        {
            base.Initialize();
            if (OutputConnections.Count == 1)
            {
                m_childFactor = GetChildNodes().First() as Factor;
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
            m_childFactor?.UpdateUtility();
            return function?.Evaluate(m_childFactor?.Utility ?? 0f) ?? 0f;
        }

        #endregion
    }
}
