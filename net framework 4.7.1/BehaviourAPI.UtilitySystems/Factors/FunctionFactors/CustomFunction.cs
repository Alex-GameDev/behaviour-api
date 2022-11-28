using System;

namespace BehaviourAPI.UtilitySystems
{
    public class CustomFunction : FunctionFactor
    {
        public Func<float, float> Func;

        public CustomFunction SetFunction(Func<float, float> func)
        {
            Func = func;
            return this;
        }

        protected override float Evaluate(float x) => Func?.Invoke(x) ?? x;
    }
}
