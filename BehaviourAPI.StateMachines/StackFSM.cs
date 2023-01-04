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

        ActionState _comeBackState;
        HashSet<Transition> _pushTransitions;

        public StackFSM()
        {
            _stateStack = new Stack<State>();
            _comeBackState = CreateActionState("comeback", new FunctionalAction(() => ReturnToLastState(), () => Status.None));
            _pushTransitions = new HashSet<Transition>();
        }

        public T CreatePopTransition<T>(string name, ActionState from, Perception perception = null, Action action = null) where T : Transition, new()
        {
            return CreateTransition<T>(name, from, _comeBackState, perception, action);
        }

        public Transition CreatePopTransition(string name, ActionState from, Perception perception = null, Action action = null)
        {
            return CreatePopTransition<Transition>(name, from, perception, action);
        }

        public T CreatePushTransition<T>(string name, ActionState from, ActionState to, Perception perception = null, Action action = null) where T : Transition, new()
        {
            T transition = CreateTransition<T>(name, from, to, perception, action);
            _pushTransitions.Add(transition);
            return transition;
        }

        public Transition CreatePushTransition(string name, ActionState from, ActionState to, Perception perception = null, Action action = null)
        {
            return CreatePushTransition<Transition>(name, from, to, perception, action);
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
