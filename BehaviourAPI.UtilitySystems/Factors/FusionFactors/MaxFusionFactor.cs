using System.Linq;

namespace BehaviourAPI.UtilitySystems
{
    public class MaxFusionFactor : FusionFactor
    {
        public override string Description => "Factor that return the maximum utility of its children";
        protected override float ComputeUtility()
        {
            m_childFactors.ForEach(f => f.UpdateUtility());
            return m_childFactors.Max(f => f.Utility);
        }
    }
}
