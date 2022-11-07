
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

        State _comeBackState;

        public StackFSM()
        {
            _stateStack = new Stack<State>();
            _comeBackState = CreateState("comeback", new FunctionalAction(() => ReturnToLastState(), () => Status.None));
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

        private void ReturnToLastState()
        {
            _stateStack.Pop(); // Ignore the last state (the origin of the transition).
            State state = _stateStack.Pop();
            if (state != null)
            {
                _currentState?.Stop();
                _currentState = state;
                _currentState?.Start();
            }
            else
                throw new NullReferenceException();
        }

        public override void SetCurrentState(State? state)
        {
            if(_currentState != null)
                _stateStack.Push(_currentState);
            base.SetCurrentState(state);
        }

        public State LastState => _stateStack.Peek();
    }
}
