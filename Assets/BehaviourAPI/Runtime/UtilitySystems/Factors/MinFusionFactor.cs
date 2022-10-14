using System.Linq;

namespace BehaviourAPI.Runtime.UtilitySystems
{

    public class MinFactor : FusionFactor
    {
        protected override float ComputeUtility()
        {
            m_childFactors.ForEach(f => f.UpdateUtility());
            return m_childFactors.Min(f => f.Utility);
        }
    }
}
