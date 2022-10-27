namespace BehaviourAPI.StateMachines
{
    using Core;
    public class FSM : BehaviourEngine
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
            return state;
        }

        public Transition CreateTransition(State from, State to)
        {
            Transition transition = CreateConnection<Transition>(from, to);
            from.AddTransition(transition);
            return transition;
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

        public override void SetCurrentNode(Node? node)
        {
            _currentState?.Stop();
            _currentState = node as State;
            _currentState?.Start();
        }

        #endregion
    }
}
