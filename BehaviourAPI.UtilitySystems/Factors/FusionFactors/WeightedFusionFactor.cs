using BehaviourAPI.Core;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.UtilitySystems
{
    public class WeightedFusionFactor : FusionFactor
    {
        public Variable<float>[] Weights = new Variable<float>[0];

        protected override float ComputeUtility()
        {
            m_childFactors.ForEach(f => f.UpdateUtility());
            return m_childFactors.Zip(Weights, (a, b) => a.Utility * b.Value).Sum();
        }

        public Factor SetWeights(params float[] weights)
        {
            Weights = weights.Map().ToArray();
            return this;
        }

        public Factor SetWeights(IEnumerable<float> weights)
        {
            Weights = weights.Map().ToArray();
            return this;
        }
    }
}
