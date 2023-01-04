﻿using System;

namespace BehaviourAPI.StateMachines
{
    using Core.Perceptions;
    using Core;
    using Core.Actions;

    public class FSM : BehaviourGraph
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type NodeType => typeof(FSMNode);

        public override bool CanRepeatConnection => false;

        public override bool CanCreateLoops => true;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        protected State _currentState;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Create a new State in this <see cref="FSM"/> that executes the <see cref="Action"/> specified in <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action this state executes.</param>
        /// <returns>The <see cref="State"/> created.</returns>
        public State CreateState(Action action = null)
        {
            State state = CreateNode<State>();
            state.Action = action;
            return state;
        }

        /// <summary>
        /// Create a new State named <paramref name="name"/> in this <see cref="FSM"/> that executes the <see cref="Action"/> specified in <paramref name="action"/>.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        /// <param name="action">The action this state executes.</param>
        /// <returns>The <see cref="State"/> created.</returns>
        public State CreateState(string name, Action action = null)
        {
            State state = CreateNode<State>(name);
            state.Action = action;
            return state;
        }



        /// <summary>
        /// Create a new State of type <typeparamref name="T"/> in this <see cref="FSM"/> that executes the given action.
        /// </summary>
        /// <param name="action">The action this state wil executes</param>
        /// <returns>The State created</returns>
        public T CreateState<T>(Action action = null) where T : State, new()
        {
            T state = CreateNode<T>();
            state.Action = action;
            return state;
        }

        /// <summary>
        /// Create a new State of type <typeparamref name="T"/> named <paramref name="name"/> in this <see cref="FSM"/> that executes the <see cref="Action"/> specified in <paramref name="action"/>.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        /// <param name="action">The action this state executes.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateState<T>(string name, Action action = null) where T : State, new()
        {
            T state = CreateNode<T>(name);
            state.Action = action;
            return state;
        }



        /// <summary>
        /// Create a new <see cref="StateTransition"/> of type <typeparamref name="T"/> named <paramref name="name"/> in this <see cref="FSM"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// The transition checks <paramref name="perception"/> and executes <paramref name="action"/> when is performed. If <paramref name="perception"/> is not specified or is null, the transition works as a lambda transition.
        /// To disable the transition from being checked from the source state, set <paramref name="isPulled"/> to false. 
        /// </summary>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="isPulled">True if the transition will be checked by its source state, false otherwise.</param>

        public StateTransition CreateTransition(string name, State from, State to, Perception perception = null, Action action = null, bool isPulled = true)
        {
            StateTransition transition = CreateNode<StateTransition>(name);
            transition.SetFSM(this);
            transition.Perception = perception;
            transition.Action = action;
            transition.isPulled = isPulled;
            Connect(from, transition);
            Connect(transition, to);
            transition.SetSourceState(from);
            transition.SetTargetState(to);
            from.AddTransition(transition);
            return transition;       
        }

        /// <summary>
        /// Create a new <see cref="StateTransition"/> of type <typeparamref name="T"/> in this <see cref="FSM"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// The transition checks <paramref name="perception"/> and executes <paramref name="action"/> when is performed. If <paramref name="perception"/> is not specified or is null, the transition works as a lambda transition.
        /// To disable the transition from being checked from the source state, set <paramref name="isPulled"/> to false. 
        /// </summary>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="isPulled">True if the transition will be checked by its source state, false otherwise.</param>
        public StateTransition CreateTransition(State from, State to, Perception perception = null, Action action = null, bool isPulled = true)
        {
            StateTransition transition = CreateNode<StateTransition>();
            transition.SetFSM(this);
            transition.Perception = perception;
            transition.Action = action;
            transition.isPulled = isPulled;
            Connect(from, transition);
            Connect(transition, to);
            transition.SetSourceState(from);
            transition.SetTargetState(to);
            from.AddTransition(transition);
            return transition;
        }



        /// <summary>
        /// Create a new <see cref="ExitTransition"/> named <paramref name="name"/> in this <see cref="FSM"/> that goes from the state <paramref name="from"/>  to exit the graph with value of <paramref name="exitStatus"/>.
        /// The transition checks <paramref name="perception"/> and executes <paramref name="action"/> when is performed. If <paramref name="perception"/> is not specified or is null, the transition works as a lambda transition.
        /// To disable the transition from being checked from the source state, set <paramref name="isPulled"/> to false. 
        /// </summary>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="isPulled">True if the transition will be checked by its source state, false otherwise.</param>
        /// <returns>The <see cref="ExitTransition"/> created.</returns>
        public Transition CreateExitTransition(string name, State from, Status exitStatus, Perception perception = null, Action action = null, bool isPulled = true)
        {
            ExitTransition transition = CreateNode<ExitTransition>(name);
            transition.SetFSM(this);
            transition.Perception = perception;
            transition.Action = action;
            transition.isPulled = isPulled;
            Connect(from, transition);
            transition.SetSourceState(from);
            from.AddTransition(transition);
            transition.ExitStatus = exitStatus;
            return transition;
        }

        /// <summary>
        /// Create a new <see cref="ExitTransition"/> in this <see cref="FSM"/> that goes from the state <paramref name="from"/> to exit the graph with value of <paramref name="exitStatus"/>.
        /// The transition checks <paramref name="perception"/> and executes <paramref name="action"/> when is performed. If <paramref name="perception"/> is not specified or is null, the transition works as a lambda transition.
        /// To disable the transition from being checked from the source state, set <paramref name="isPulled"/> to false. 
        /// </summary>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="isPulled">True if the transition will be checked by its source state, false otherwise.</param>
        /// <returns>The <see cref="ExitTransition"/> created.</returns>
        public Transition CreateExitTransition(State from, Status exitStatus, Perception perception = null, Action action = null, bool isPulled = true)
        {
            ExitTransition transition = CreateNode<ExitTransition>();
            transition.SetFSM(this);
            transition.Perception = perception;
            transition.Action = action;
            transition.isPulled = isPulled;
            Connect(from, transition);
            transition.SetSourceState(from);
            from.AddTransition(transition);
            transition.ExitStatus = exitStatus;
            return transition;
        }



        /// <summary>
        /// Create a new <see cref="Transition"/> of type <typeparamref name="T"/> named <paramref name="name"/> in this <see cref="FSM"/> that goes from the probabilistic state <paramref name="from"/> to the state <paramref name="to"/>, and has a probability of being checked
        /// every iteration specified in <paramref name="probability"/>.
        /// The transition checks <paramref name="perception"/> and executes <paramref name="action"/> when is performed. If <paramref name="perception"/> is not specified or is null, the transition works as a lambda transition.
        /// To disable the transition from being checked from the source state, set <paramref name="isPulled"/> to false. 
        /// </summary>
        /// <typeparam name="T">The type of the transition.</typeparam>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source probabilistic state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="probability">The probability of being checked.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="isPulled">True if the transition will be checked by its source state, false otherwise.</param>
        public StateTransition CreateProbabilisticTransition(string name, ProbabilisticState from, State to, float probability, Perception perception = null, Action action = null, bool isPulled = true)
        {
            StateTransition transition = CreateTransition(name, from, to, perception, action, isPulled);
            from.SetProbabilisticTransition(transition, probability);
            return transition;
        }

        /// <summary>
        /// Create a new <see cref="Transition"/> of type <typeparamref name="T"/> in this <see cref="FSM"/> that goes from the probabilistic state <paramref name="from"/> to the state <paramref name="to"/>, and has a probability of being checked
        /// every iteration specified in <paramref name="probability"/>.
        /// The transition checks <paramref name="perception"/> and executes <paramref name="action"/> when is performed. If <paramref name="perception"/> is not specified or is null, the transition works as a lambda transition.
        /// To disable the transition from being checked from the source state, set <paramref name="isPulled"/> to false. 
        /// </summary>
        /// <param name="from">The source probabilistic state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="probability">The probability of being checked.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="isPulled">True if the transition will be checked by its source state, false otherwise.</param>
        public StateTransition CreateProbabilisticTransition(ProbabilisticState from, State to, float probability, Perception perception = null, Action action = null, bool isPulled = true)
        {
            StateTransition transition = CreateTransition(from, to, perception, action, isPulled);
            from.SetProbabilisticTransition(transition, probability);
            return transition;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public void SetEntryState(State state)
        {
            StartNode = state;
        }

        public override void Start()
        {
            base.Start();

            _currentState = StartNode as State;
            _currentState?.Start();
        }

        public override void Execute()
        {
            _currentState?.Update();
        }

        public override void Stop()
        {
            base.Stop();
            _currentState?.Stop();
        }

        public virtual void SetCurrentState(State state)
        {
            _currentState?.Stop();
            _currentState = state;
            _currentState?.Start();
        }

        public virtual void OnTriggerTransition(Transition transition) { }

        public bool IsCurrentState(State state) => _currentState == state;

        #endregion
    }
}
