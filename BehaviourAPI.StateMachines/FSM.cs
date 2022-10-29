namespace BehaviourAPI.StateMachines
{
    using Core.Perceptions;
    using Core;
    public class FSM : BehaviourGraph
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type NodeType => typeof(State);
        public override Type ConnectionType => typeof(Transition);

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        State? _entryState;

        State? _currentState;        

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public T CreateState<T>(string name) where T : State
        {
            T state = CreateState<T>(name);
            state.SetFSM(this);
            return state;
        }

        public T CreateTransition<T>(State from, State to, Perception perception) where T : Transition, new()
        {
            T transition = CreateConnection<T>(from, to);
            transition.Perception = perception;
            transition.SetTargetState(to);
            from.AddTransition(transition);
            return transition;
        }

        public T CreateFinishStateTransition<T>(State from, State to, bool triggerOnSuccess, bool triggerOnFailure) where T : Transition, new()
        {
            Perception finishStatePerception = new FinishExecutionPerception(from, triggerOnSuccess, triggerOnFailure); 
            return CreateTransition<T>(from, to, finishStatePerception);
        }

        public override bool SetStartNode(Node node)
        {
            bool starNodeUpdated = base.SetStartNode(node);
            if (starNodeUpdated) _entryState = node as State;
            return starNodeUpdated;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            _currentState = _entryState;
            _currentState?.Start();
        }

        public override void Update()
        {
            _currentState?.Update();
        }

        public override void Stop()
        {
            _currentState?.Stop();
        }

        public void SetCurrentState(State? state)
        {
            _currentState?.Stop();
            _currentState = state;
            _currentState?.Start();
        }

        #endregion
    }
}
