namespace BehaviourAPI.UtilitySystems
{
    public class MaxFusionFactor : FusionFactor
    {
        protected override float ComputeUtility()
        {
            m_childFactors.ForEach(f => f.UpdateUtility());
            return m_childFactors.Max(f => f.Utility);
        }
    }
}
