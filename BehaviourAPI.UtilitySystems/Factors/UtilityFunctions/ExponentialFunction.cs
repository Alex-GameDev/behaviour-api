namespace BehaviourAPI.UtilitySystems
{
    public class ExponentialFunction : UtilityFunction
    {
        public float Exp, DespX, DespY;

        public ExponentialFunction(float exp = 1, float despX = 0, float despY = 0)
        {
            Exp = exp;
            DespX = despX;
            DespY = despY;
        }

        public override float Evaluate(float x) => Math.Clamp(MathF.Pow(x - DespX, Exp) + DespY, 0f, 1f);
    }
}
