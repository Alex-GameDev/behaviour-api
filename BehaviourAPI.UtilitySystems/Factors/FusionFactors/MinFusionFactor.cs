using System.Linq;

namespace BehaviourAPI.UtilitySystems
{
    public class MinFusionFactor : FusionFactor
    {
        public override string Description => "Factor that return the minimum utility of its children";
        protected override float ComputeUtility()
        {
            m_childFactors.ForEach(f => f.UpdateUtility());
            return m_childFactors.Min(f => f.Utility);
        }
    }
}
