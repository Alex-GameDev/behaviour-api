
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.StateMachines
{
    using BehaviourAPI.Core;
    using BehaviourAPI.Core.Actions;
    using Core.Perceptions;
    public class StackFSM : FSM
    {
        Stack<State> _stateStack;

        ActionState _comeBackState;

        public StackFSM()
        {
            _stateStack = new Stack<State>();
            _comeBackState = CreateState<ActionState>("comeback").SetAction(new FunctionalAction(ReturnToLastState));
        }

        public T CreateComebackTransition<T>(string name, State from, Perception perception) where T : Transition, new()
        {
            return CreateTransition<T>(name, from, _comeBackState, perception);
        }

        public override void Stop()
        {
            base.Stop();
            _stateStack.Clear();
        }

        private Status ReturnToLastState()
        {
            State state = _stateStack.Pop();
            if (state != null)
            {
                _currentState?.Stop();
                _currentState = state;
                _currentState?.Start();
                return Status.Sucess;
            }
            else
                return Status.Error;
        }

        public override void SetCurrentState(State? state)
        {
            if(_currentState != null)
                _stateStack.Push(_currentState);
            SetCurrentState(state);
        }
    }
}
