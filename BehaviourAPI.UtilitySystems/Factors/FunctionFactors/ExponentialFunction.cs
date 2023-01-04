namespace BehaviourAPI.UtilitySystems
{
    using Core;
    using System;

    public class ExponentialFunction : FunctionFactor
    {
        public override string Description => "Factor that returns the result of applying a exponential function to its child utility";

        public float Exp, DespX, DespY;

        public ExponentialFunction SetExponent(float exp)
        {
            Exp = exp;
            return this;
        }

        public ExponentialFunction SetDespX(float despX)
        {
            DespX = despX;
            return this;
        }

        public ExponentialFunction SetDespY(float despY)
        {
            DespY = despY;
            return this;
        }

        protected override float Evaluate(float x) => MathUtilities.Clamp01((float)Math.Pow(x - DespX, Exp) + DespY);
    }
}
