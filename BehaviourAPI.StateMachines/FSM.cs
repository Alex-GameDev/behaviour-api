namespace BehaviourAPI.StateMachines
{
    using Core.Perceptions;
    using Core;
    using Core.Actions;

    public class FSM : BehaviourGraph
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type NodeType => typeof(State);

        public override Type ConnectionType => typeof(Transition);

        public override bool CanRepeatConnection => true;

        public override bool CanCreateLoops => true;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        protected State? _currentState;

        protected Dictionary<string, Transition> _transitionDict = new Dictionary<string, Transition>();

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public State CreateState(string name, Action? action = null)
        {
            State state = CreateNode<State>(name);
            state.Action = action;
            return state;
        }

        public T CreateState<T>(string name, Action? action = null) where T : State, new()
        {
            T state = CreateNode<T>(name);
            state.Action = action;
            return state;
        }

        public T CreateTransition<T>(string name, State from, State to, Perception? perception = null, Action? action = null) where T : Transition, new()
        {
            if (!_transitionDict.ContainsKey(name))
            {
                T transition = CreateConnection<T>(from, to);
                transition.Perception = perception;
                transition.Action = action;
                transition.SetFSM(this);
                transition.SetSourceState(from);
                transition.SetTargetState(to);
                from.AddTransition(transition);
                _transitionDict.Add(name, transition);
                return transition;
            }
            else
            {
                throw new ArgumentException(name, "This FSM already contains a transition with this name.");
            }           
        }
        
        public Transition CreateTransition(string name, State from, State to, Perception? perception = null, Action? action = null)
        {
            return CreateTransition<Transition>(name, from, to, perception, action);
        }

        public T CreateProbabilisticTransition<T>(string name, ProbabilisticState from, State to, float probability, Perception? perception = null) where T : Transition, new()
        {
            T transition = CreateTransition<T>(name, from, to, perception);
            from.SetProbabilisticTransition(transition, probability);
            return transition;
        }

        public Transition CreateProbabilisticTransition(string name, ProbabilisticState from, State to, float probability, Perception? perception = null) 
        {
            return CreateProbabilisticTransition<Transition>(name, from, to, probability, perception);
        }

        public T CreateFinishStateTransition<T>(string name, State from, State to, bool triggerOnSuccess, bool triggerOnFailure, Action? action = null) where T : Transition, new()
        {
            Perception finishStatePerception = new ExecutionStatusPerception(from, triggerOnSuccess, triggerOnFailure); 
            return CreateTransition<T>(name, from, to, finishStatePerception, action);
        }

        public Transition CreateFinishStateTransition(string name, State from, State to, bool triggerOnSuccess, bool triggerOnFailure, Action? action = null)
        {
            Perception finishStatePerception = new ExecutionStatusPerception(from, triggerOnSuccess, triggerOnFailure);
            return CreateTransition<Transition>(name, from, to, finishStatePerception, action);
        }

        public Transition? FindTransition(string name)
        {
            if (_transitionDict.TryGetValue(name, out Transition? transition))
            {
                return transition;
            }
            else
            {
                throw new KeyNotFoundException($"Node \"{name}\" doesn't exist in this graph");
            }
        }

        public T? FindTransition<T>(string name) where T : Transition
        {
            if (_transitionDict.TryGetValue(name, out Transition? transition))
            {
                if (transition is T element)
                {
                    return element;
                }
                else
                {
                    throw new InvalidCastException($"Transition \"{name}\" exists, but is not an instance of {typeof(T).FullName} class.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Transition \"{name}\" doesn't exist in this fsm.");
            }
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

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

        public virtual void SetCurrentState(State? state)
        {
            _currentState?.Stop();
            _currentState = state;
            _currentState?.Start();
        }

        public virtual void OnTriggerTransition(Transition transition) { }

        public bool IsCurrentState(State? state) => _currentState == state;

        #endregion
    }
}
