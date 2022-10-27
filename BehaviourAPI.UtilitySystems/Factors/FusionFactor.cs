namespace BehaviourAPI.UtilitySystems
{
    public abstract class FusionFactor : Factor
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override int MaxOutputConnections => -1;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public FusionFactor()
        {
            m_childFactors = new List<Factor?>();
        }

        public void AddFactor(Factor factor) => m_childFactors.Add(factor);

        public override void Initialize()
        {
            base.Initialize();
            GetChildNodes().ToList().ForEach(node => m_childFactors.Add(node as Factor));
        }
        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        protected List<Factor?> m_childFactors;

        #endregion
    }
}