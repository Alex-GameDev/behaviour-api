namespace BehaviourAPI.StateMachines
{
    using Core;
    public abstract class State : Node, IStatusHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type ChildType => typeof(State);

        public Status Status { get; set; }

        #endregion

        #region -------------------------------------------- Fields ------------------------------------------

        List<Transition?> _transitions;

        private FSM? _fSM;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public State()
        {
            _transitions = new List<Transition?>();
        }

        public void AddTransition(Transition transition) => _transitions.Add(transition);

        public void SetFSM(FSM fsm) => _fSM = fsm;
        
        public override void Initialize()
        {
            base.Initialize();
            OutputConnections.ForEach(conn => _transitions.Add(conn as Transition));
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public virtual void Start()
        {
            Status = Status.Running;
            _transitions.ForEach(t => t?.Start());
        }

        public void Update()
        {
            if (CheckTransitions()) return;
            Status = UpdateStatus();
        }

        public virtual void Stop()
        {
            Status = Status.None;
            _transitions.ForEach(t => t?.Stop());
        }

        private bool CheckTransitions()
        {
            for(int i = 0; i < _transitions.Count; i++)
            {
                bool check = _transitions[i]?.Check() ?? false;
                if(check)
                {
                    _fSM?.SetCurrentState(_transitions[i]?.GetTargetState());
                    return true;
                }
            }
            return false;
        }

        protected abstract Status UpdateStatus();

        #endregion
    }
}
