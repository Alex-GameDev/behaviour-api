namespace BehaviourAPI.UtilitySystems
{
    using Core;
    public class LinearFunction : FunctionFactor
    {
        public float slope, yIntercept;

        public LinearFunction SetSlope(float slope)
        {
            this.slope = slope;
            return this;
        }

        public LinearFunction SetYIntercept(float yIntercept)
        {
            this.yIntercept = yIntercept;
            return this;
        }

        protected override float Evaluate(float x) => MathUtilities.Clamp01(slope * x + yIntercept);
    }
}
