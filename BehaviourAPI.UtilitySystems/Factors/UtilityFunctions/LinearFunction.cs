namespace BehaviourAPI.UtilitySystems
{
    public class LinearFunction : UtilityFunction
    {
        public float m, n;

        public LinearFunction(float m, float n)
        {
            this.m = m;
            this.n = n;
        }

        public override float Evaluate(float x) => Math.Clamp(m * x + n, 0f, 1f);
    }
}
