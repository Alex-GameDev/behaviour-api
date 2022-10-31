using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.UtilitySystems
{
    public class CustomFunction : UtilityFunction
    {
        public Func<float, float>? Func;

        public CustomFunction(Func<float, float> func)
        {
            Func = func;
        }

        public override float Evaluate(float x) => Func?.Invoke(x) ?? x;
    }
}
