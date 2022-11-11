using BehaviourAPI.Core.Perceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.StateMachines
{
    public class ProbabilisticState : State
    {
        Dictionary<Transition, float> _probabilities;
        float _totalProbability;
        Random _random;

        public double Prob { get; private set; }
        public ProbabilisticState()
        {
            _probabilities = new Dictionary<Transition, float>();
            _random = new Random();
        }
        
        public void SetProbabilisticTransition(Transition transition, float probability)
        {
            if(_transitions.Contains(transition))
            {
                _probabilities[transition] = probability;
                _totalProbability = MathF.Max(_probabilities.Sum(p => p.Value), 1f);
            }            
        }

        protected override bool CheckTransitions()
        {
            var probability = _random.NextDouble() * _totalProbability;
            Prob = probability;
            var currentProbSum = 0f;
            Transition? selectedTransition = null;
            for (int i = 0; i < _transitions.Count; i++)
            {
                Transition? transition = _transitions[i];
                if (transition == null) break;
                
                if(_probabilities.TryGetValue(transition, out float value))
                {
                    if (selectedTransition == null)
                    {
                        currentProbSum += value;
                        if (currentProbSum > probability)
                        {
                            selectedTransition = transition;
                        }
                    }
                }
                else
                {
                    if (transition.Check())
                    {
                        _transitions[i]?.Perform();
                        return true;
                    }
                }
            }
            if (selectedTransition != null)
            {
                if(selectedTransition.Perception == null || selectedTransition.Check())
                {
                    selectedTransition.Perform();
                    return true;
                    
                }                    
            }
            return false;
            
        }

        public float GetProbability(Transition t)
        {
            return _probabilities[t];
        }
    }
}
