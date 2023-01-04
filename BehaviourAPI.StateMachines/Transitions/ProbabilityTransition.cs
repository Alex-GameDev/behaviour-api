using BehaviourAPI.Core;
using BehaviourAPI.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.StateMachines
{
    public class ProbabilityTransition : Transition
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override int MaxOutputConnections => -1;

        public float[] Probabilities = new float[0];

        protected List<State> _targetStates;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public ProbabilityTransition SetProbabilities(params float[] probabilities)
        {
            Probabilities = probabilities;
            return this;
        }

        public ProbabilityTransition SetWeights(IEnumerable<float> probabilities)
        {
            Probabilities = probabilities.ToArray();
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Perform()
        {
            base.Perform();

            var randomValue = MathUtilities.Random.NextDouble() * Math.Max(Probabilities.Sum(), 1f);

            State selectedState = null;
            var currentProb = 0f;
            int i = 0;
            while(selectedState == null && i < _targetStates.Count && i < Probabilities.Count())
            {
                currentProb += Probabilities[i];
                if(currentProb > randomValue)
                {
                    selectedState = _targetStates[i];
                }
            }

            if (selectedState == null)
                throw new MissingChildException(this, "The list of utility candidates is empty.");

            _fsm.SetCurrentState(selectedState);
        }

        #endregion
    }
}
