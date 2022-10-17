using System.Linq;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    public class WeightedFusionFactor : FusionFactor
    {
        public override string Name => "Weighted Fusion Factor";
        public override string Description => "Fusion factor that returns the weighted sum of its childs";
        public float[] Weights;
        protected override float ComputeUtility()
        {
            m_childFactors.ForEach(f => f.UpdateUtility());
            return m_childFactors.Zip(Weights, (a, b) => a.Utility * b).Sum();
        }
    }
}
