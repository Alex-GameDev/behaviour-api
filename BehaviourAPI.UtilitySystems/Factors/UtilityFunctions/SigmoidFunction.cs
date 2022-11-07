namespace BehaviourAPI.UtilitySystems
{
    public class SigmoidFunction : UtilityFunction
    {
        public float a, b;

        public SigmoidFunction(float a, float b)
        {
            this.a = a;
            this.b = b;
        }

        public override float Evaluate(float x) => Math.Clamp(1f / (1f + MathF.Pow(MathF.E, -a * (x - b))), 0f, 1f);
    }
}
