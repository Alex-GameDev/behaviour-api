using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.StateMachines
{
    /// <summary>
    /// Transition that execute a parameterless method when is performed
    /// </summary>
    public class MealyTransition : Transition
    {
        Action? _onPerformAction;

        public MealyTransition SetOnPerformAction(Action action)
        {
            _onPerformAction = action;
            return this;
        }

        public override void Perform()
        {
            _onPerformAction?.Invoke();
            base.Perform();
        }
    }
}
