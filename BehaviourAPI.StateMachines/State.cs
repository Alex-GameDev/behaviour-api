namespace BehaviourAPI.StateMachines
{
    using Core;
    using Core.Actions;

    public class State : Node, IStatusHandler, IActionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override int MaxInputConnections => -1;
        public override int MaxOutputConnections => -1;
        public override Type ChildType => typeof(State);

        public Status Status { get => _status; protected set => _status = value; }
        Status _status;

        public Action? Action { get => _action; set => _action = value; }       
        Action? _action;

        #endregion

        #region -------------------------------------------- Fields ------------------------------------------

        List<Transition?> _transitions;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public State()
        {
            _transitions = new List<Transition?>();
        }

        public void AddTransition(Transition transition) => _transitions.Add(transition);
       
        public override void Initialize()
        {
            base.Initialize();
            OutputConnections.ForEach(conn => _transitions.Add(conn as Transition));
        }

        public State SetAction(Action? action)
        {
            Action = action;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public virtual void Start()
        {
            Status = Status.Running;
            _transitions.ForEach(t => t?.Start());
            Action?.Start();
        }

        public void Update()
        {
            if (CheckTransitions()) return;
            Status = Action?.Update() ?? Status.Error;
        }

        public virtual void Stop()
        {
            Status = Status.None;
            _transitions.ForEach(t => t?.Stop());
            Action?.Stop();
        }

        private bool CheckTransitions()
        {
            for(int i = 0; i < _transitions.Count; i++)
            {
                bool check = _transitions[i]?.Check() ?? false;
                if(check)
                {
                    _transitions[i]?.Perform();
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
