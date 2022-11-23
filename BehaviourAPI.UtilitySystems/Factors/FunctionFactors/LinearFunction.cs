namespace BehaviourAPI.UtilitySystems
{
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

        protected override float Evaluate(float x) => Math.Clamp(slope * x + yIntercept, 0f, 1f);
    }
}
