using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.UtilitySystems
{
    public class LinearFunction : UtilityFunction
    {
        public float m, n;
        public override float Evaluate(float x) => m * x + n;
    }
}
