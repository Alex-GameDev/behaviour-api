namespace BehaviourAPI.UtilitySystems
{
    using BehaviourAPI.UtilitySystems;
    using Core;

    /// <summary>
    /// Factor that modifies its child value with a function.
    /// </summary>  
    public abstract class FunctionFactor : Factor
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override int MaxOutputConnections => 1;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

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
            if (Children.Count == 1)
            {
                m_childFactor = GetFirstChild() as Factor;
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
            return Evaluate(m_childFactor?.Utility ?? 0f);
        }

        protected abstract float Evaluate(float childUtility);
        #endregion
    }
}
