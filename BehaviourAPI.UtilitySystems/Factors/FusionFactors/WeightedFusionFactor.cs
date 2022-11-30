using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.UtilitySystems
{
    public class WeightedFusionFactor : FusionFactor
    {
        public float[] Weights = new float[0];

        protected override float ComputeUtility()
        {
            m_childFactors.ForEach(f => f.UpdateUtility());
            return m_childFactors.Zip(Weights, (a, b) => a.Utility * b).Sum();
        }

        public Factor SetWeights(params float[] weights)
        {
            Weights = weights;
            return this;
        }

        public Factor SetWeights(List<float> weights)
        {
            Weights = weights.ToArray();
            return this;
        }
    }
}
