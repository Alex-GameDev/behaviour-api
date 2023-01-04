using System;

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
        /// <returns>The <see cref="ActionState"/> created.</returns>
        public ActionState CreateActionState(Action action = null)
        {
            ActionState state = CreateNode<ActionState>();
            state.Action = action;
            return state;
        }

        /// <summary>
        /// Create a new State named <paramref name="name"/> in this <see cref="FSM"/> that executes the <see cref="Action"/> specified in <paramref name="action"/>.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        /// <param name="action">The action this state executes.</param>
        /// <returns>The <see cref="ActionState"/> created.</returns>
        public ActionState CreateActionState(string name, Action action = null)
        {
            ActionState state = CreateNode<ActionState>(name);
            state.Action = action;
            return state;
        }

        /// <summary>
        /// Create a new State of type <typeparamref name="T"/> in this <see cref="FSM"/> that executes the given action.
        /// </summary>
        /// <param name="action">The action this state wil executes</param>
        /// <returns>The State created</returns>
        public T CreateActionState<T>(Action action = null) where T : ActionState, new()
        {
            T state = CreateNode<T>();
            state.Action = action;
            return state;
        }

        /// <summary>
        /// Create a new State named <paramref name="name"/> in this <see cref="FSM"/> that exits the graph execution with value of <paramref name="exitStatus"/>.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        /// <param name="exitStatus">The exit value.</param>
        public ExitState CreateExitState(string name, Status exitStatus)
        {
            ExitState state = CreateNode<ExitState>(name);
            state.ExitStatus = exitStatus;
            return state;
        }

        /// <summary>
        /// Create a new State named <paramref name="name"/> in this <see cref="FSM"/> that exits the graph execution with value of <paramref name="exitStatus"/>.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        /// <param name="exitStatus">The exit value.</param>
        public ExitState CreateExitState(Status exitStatus)
        {
            ExitState state = CreateNode<ExitState>();
            state.ExitStatus = exitStatus;
            return state;
        }

        /// <summary>
        /// Create a new State of type <typeparamref name="T"/> named <paramref name="name"/> in this <see cref="FSM"/> that executes the <see cref="Action"/> specified in <paramref name="action"/>.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        /// <param name="action">The action this state executes.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateActionState<T>(string name, Action action = null) where T : ActionState, new()
        {
            T state = CreateNode<T>(name);
            state.Action = action;
            return state;
        }

        /// <summary>
        /// Create a new <see cref="Transition"/> of type <typeparamref name="T"/> named <paramref name="name"/> in this <see cref="FSM"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// The transition checks <paramref name="perception"/> and executes <paramref name="action"/> when is performed. If <paramref name="perception"/> is not specified or is null, the transition works as a lambda transition.
        /// To disable the transition from being checked from the source state, set <paramref name="isPulled"/> to false. 
        /// </summary>
        /// <typeparam name="T">The type of the transition.</typeparam>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="isPulled">True if the transition will be checked by its source state, false otherwise.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateTransition<T>(string name, ActionState from, State to, Perception perception = null, Action action = null, bool isPulled = true) where T : Transition, new()
        {
            T transition = CreateNode<T>(name);
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
        /// Create a new <see cref="Transition"/> of type <typeparamref name="T"/> in this <see cref="FSM"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// The transition checks <paramref name="perception"/> and executes <paramref name="action"/> when is performed. If <paramref name="perception"/> is not specified or is null, the transition works as a lambda transition.
        /// To disable the transition from being checked from the source state, set <paramref name="isPulled"/> to false. 
        /// </summary>
        /// <typeparam name="T">The type of the transition.</typeparam>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="isPulled">True if the transition will be checked by its source state, false otherwise.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateTransition<T>(ActionState from, State to, Perception perception = null, Action action = null, bool isPulled = true) where T : Transition, new()
        {
            T transition = CreateNode<T>();
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
        /// Create a new <see cref="Transition"/> named <paramref name="name"/> in this <see cref="FSM"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// The transition checks <paramref name="perception"/> and executes <paramref name="action"/> when is performed. If <paramref name="perception"/> is not specified or is null, the transition works as a lambda transition.
        /// To disable the transition from being checked from the source state, set <paramref name="isPulled"/> to false. 
        /// </summary>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="isPulled">True if the transition will be checked by its source state, false otherwise.</param>
        /// <returns>The <see cref="Transition"/> created.</returns>
        public Transition CreateTransition(string name, ActionState from, State to, Perception perception = null, Action action = null, bool isPulled = true)
        {
            return CreateTransition<Transition>(name, from, to, perception, action, isPulled);
        }

        /// <summary>
        /// Create a new <see cref="Transition"/> in this <see cref="FSM"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// The transition checks <paramref name="perception"/> and executes <paramref name="action"/> when is performed. If <paramref name="perception"/> is not specified or is null, the transition works as a lambda transition.
        /// To disable the transition from being checked from the source state, set <paramref name="isPulled"/> to false. 
        /// </summary>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="isPulled">True if the transition will be checked by its source state, false otherwise.</param>
        /// <returns>The <see cref="Transition"/> created.</returns>
        public Transition CreateTransition(ActionState from, State to, Perception perception = null, Action action = null, bool isPulled = true)
        {
            return CreateTransition<Transition>(from, to, perception, action, isPulled);
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
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateProbabilisticTransition<T>(string name, ProbabilisticState from, State to, float probability, Perception perception = null, Action action = null, bool isPulled = true) where T : Transition, new()
        {
            T transition = CreateTransition<T>(name, from, to, perception, action, isPulled);
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
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateProbabilisticTransition<T>(ProbabilisticState from, State to, float probability, Perception perception = null, Action action = null, bool isPulled = true) where T : Transition, new()
        {
            T transition = CreateTransition<T>(from, to, perception, action, isPulled);
            from.SetProbabilisticTransition(transition, probability);
            return transition;
        }

        /// <summary>
        /// Create a new <see cref="Transition"/> named <paramref name="name"/> in this <see cref="FSM"/> that goes from the probabilistic state <paramref name="from"/> to the state <paramref name="to"/>, and has a probability of being checked
        /// every iteration specified in <paramref name="probability"/>.
        /// The transition checks <paramref name="perception"/> and executes <paramref name="action"/> when is performed. If <paramref name="perception"/> is not specified or is null, the transition works as a lambda transition.
        /// To disable the transition from being checked from the source state, set <paramref name="isPulled"/> to false. 
        /// </summary>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source probabilistic state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="probability">The probability of being checked.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="isPulled">True if the transition will be checked by its source state, false otherwise.</param>
        /// <returns>The <see cref="Transition"/> created.</returns>
        public Transition CreateProbabilisticTransition(string name, ProbabilisticState from, State to, float probability, Perception perception = null, Action action = null, bool isPulled = true) 
        {
            return CreateProbabilisticTransition<Transition>(name, from, to, probability, perception, action, isPulled);
        }

        /// <summary>
        /// Create a new <see cref="Transition"/>  in this <see cref="FSM"/> that goes from the probabilistic state <paramref name="from"/> to the state <paramref name="to"/>, and has a probability of being checked
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
        /// <returns>The <see cref="Transition"/> created.</returns>
        public Transition CreateProbabilisticTransition(ProbabilisticState from, State to, float probability, Perception perception = null, Action action = null, bool isPulled = true)
        {
            return CreateProbabilisticTransition<Transition>(from, to, probability, perception, action, isPulled);
        }

        /// <summary>
        /// Create a new <see cref="Transition"/> of type <typeparamref name="T"/> named <paramref name="name"/> in this <see cref="FSM"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// The transition checks the <paramref name="from"/>'s <see cref="Status"/> and triggers when it execution ends. If <paramref name="triggerOnSuccess"/> is true, it will be activated when <paramref name="from"/> execution ends with <value>Status.Success</value> 
        /// and if <paramref name="triggerOnFailure"/> is true, when ends with <value>Status.Failure</value> 
        /// </summary>
        /// <typeparam name="T">The type of the transition.</typeparam>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateFinishStateTransition<T>(string name, ActionState from, State to, bool triggerOnSuccess, bool triggerOnFailure, Action action = null) where T : Transition, new()
        {
            Perception finishStatePerception = new ExecutionStatusPerception(from, triggerOnSuccess, triggerOnFailure); 
            return CreateTransition<T>(name, from, to, finishStatePerception, action);
        }

        /// <summary>
        /// Create a new <see cref="Transition"/> of type <typeparamref name="T"/> in this <see cref="FSM"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// The transition checks the <paramref name="from"/>'s <see cref="Status"/> and triggers when it execution ends. If <paramref name="triggerOnSuccess"/> is true, it will be activated when <paramref name="from"/> execution ends with <value>Status.Success</value> 
        /// and if <paramref name="triggerOnFailure"/> is true, when ends with <value>Status.Failure</value> 
        /// </summary>
        /// <typeparam name="T">The type of the transition.</typeparam>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateFinishStateTransition<T>(ActionState from, State to, bool triggerOnSuccess, bool triggerOnFailure, Action action = null) where T : Transition, new()
        {
            Perception finishStatePerception = new ExecutionStatusPerception(from, triggerOnSuccess, triggerOnFailure);
            return CreateTransition<T>(from, to, finishStatePerception, action);
        }

        /// <summary>
        /// Create a new <see cref="Transition"/> named <paramref name="name"/> in this <see cref="FSM"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// The transition checks the <paramref name="from"/>'s <see cref="Status"/> and triggers when it execution ends. If <paramref name="triggerOnSuccess"/> is true, it will be activated when <paramref name="from"/> execution ends with <value>Status.Success</value> 
        /// and if <paramref name="triggerOnFailure"/> is true, when ends with <value>Status.Failure</value> 
        /// </summary>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <returns>The <see cref="Transition"/> created.</returns>
        public Transition CreateFinishStateTransition(string name, ActionState from, State to, bool triggerOnSuccess, bool triggerOnFailure, Action action = null)
        {
            Perception finishStatePerception = new ExecutionStatusPerception(from, triggerOnSuccess, triggerOnFailure);
            return CreateTransition<Transition>(name, from, to, finishStatePerception, action);
        }

        /// <summary>
        /// Create a new <see cref="Transition"/> in this <see cref="FSM"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// The transition checks the <paramref name="from"/>'s <see cref="Status"/> and triggers when it execution ends. If <paramref name="triggerOnSuccess"/> is true, it will be activated when <paramref name="from"/> execution ends with <value>Status.Success</value> 
        /// and if <paramref name="triggerOnFailure"/> is true, when ends with <value>Status.Failure</value> 
        /// </summary>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <returns>The <see cref="Transition"/> created.</returns>
        public Transition CreateFinishStateTransition(ActionState from, State to, bool triggerOnSuccess, bool triggerOnFailure, Action action = null)
        {
            Perception finishStatePerception = new ExecutionStatusPerception(from, triggerOnSuccess, triggerOnFailure);
            return CreateTransition<Transition>(from, to, finishStatePerception, action);
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

            _currentState = StartNode as ActionState;
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
