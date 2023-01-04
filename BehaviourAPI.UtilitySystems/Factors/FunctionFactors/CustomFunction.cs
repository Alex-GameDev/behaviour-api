using System;

namespace BehaviourAPI.UtilitySystems
{
    public class CustomFunction : FunctionFactor
    {
        public override string Description => "Factor that returns the result of applying a custom function to its child utility";

        public Func<float, float> Func;

        public CustomFunction SetFunction(Func<float, float> func)
        {
            Func = func;
            return this;
        }

        protected override float Evaluate(float x) => Func?.Invoke(x) ?? x;
    }
}
