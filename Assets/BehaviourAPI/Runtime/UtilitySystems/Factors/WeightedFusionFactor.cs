using System.Linq;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    public class WeightedFusionFactor : FusionFactor
    {
        public float[] Weights;
        protected override float ComputeUtility()
        {
            m_childFactors.ForEach(f => f.UpdateUtility());
            return m_childFactors.Zip(Weights, (a, b) => a.Utility * b).Sum();
        }
    }
}
