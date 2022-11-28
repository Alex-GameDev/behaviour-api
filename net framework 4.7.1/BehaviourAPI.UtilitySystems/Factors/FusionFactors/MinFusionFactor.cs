using System.Linq;

namespace BehaviourAPI.UtilitySystems
{
    public class MinFusionFactor : FusionFactor
    {
        protected override float ComputeUtility()
        {
            m_childFactors.ForEach(f => f.UpdateUtility());
            return m_childFactors.Min(f => f.Utility);
        }
    }
}
