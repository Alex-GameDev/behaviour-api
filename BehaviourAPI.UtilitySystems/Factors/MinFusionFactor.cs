namespace BehaviourAPI.UtilitySystems
{
    public class MinFusionFactor : FusionFactor
    {
        public override string Description => "Fusion factor that returns the minimum value of its childs";

        protected override float ComputeUtility()
        {
            m_childFactors.ForEach(f => f.UpdateUtility());
            return m_childFactors.Min(f => f.Utility);
        }
    }
}
