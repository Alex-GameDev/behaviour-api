using BehaviourAPI.Core;
using System;

namespace BehaviourAPI.UtilitySystems
{
    public class SigmoidFunction : FunctionFactor
    {
        public float grownRate, midpoint;

        public SigmoidFunction SetValues(float grownRate, float midpoint)
        {
            this.grownRate = grownRate;
            this.midpoint = midpoint;
            return this;
        }

        public SigmoidFunction SetGrownRate(float grownRate)
        {
            this.grownRate = grownRate;
            return this;
        }

        public SigmoidFunction SetMidpoint(float midpoint)
        {

            this.midpoint = midpoint;
            return this;
        }

        protected override float Evaluate(float x) => MathUtilities.Clamp01((float)(1f / (1f + Math.Pow(Math.E, -grownRate * (x - midpoint)))));
    }
}
