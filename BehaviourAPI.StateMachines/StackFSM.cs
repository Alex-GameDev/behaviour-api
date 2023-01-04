using System.Collections.Generic;
using System;

namespace BehaviourAPI.StateMachines
{
    using BehaviourAPI.Core;
    using BehaviourAPI.Core.Actions;
    using Core.Perceptions;
    using Action = Core.Actions.Action;

    public class StackFSM : FSM
    {
        Stack<State> _stateStack;

        State _comeBackState;
        HashSet<Transition> _pushTransitions;

        public StackFSM()
        {
            _stateStack = new Stack<State>();
            _comeBackState = CreateState("comeback", new FunctionalAction(() => ReturnToLastState(), () => Status.None));
            _pushTransitions = new HashSet<Transition>();
        }

        public StateTransition CreatePopTransition(string name, State from, Perception perception = null, Action action = null)
        {
            return CreateTransition(name, from, _comeBackState, perception, action);
        }

        public StateTransition CreatePushTransition(string name, State from, State to, Perception perception = null, Action action = null)
        {
            StateTransition transition = CreateTransition(name, from, to, perception, action);
            _pushTransitions.Add(transition);
            return transition;
        }

        public override void Stop()
        {
            base.Stop();
            _stateStack.Clear();
        }

        private void ReturnToLastState()
        {
            // Ignore the last state (the origin of the transition).
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

        public override void OnTriggerTransition(Transition transition) 
        { 
            if(_pushTransitions.Contains(transition) && _currentState != null)
              _stateStack.Push(_currentState);
        }

        public State LastState => _stateStack.Peek();
    }
}
