namespace BehaviourAPI.StateMachines
{
    using BehaviourAPI.Core.Actions;
    using BehaviourAPI.Core.Perceptions;
    using Core;

    public class Transition : Connection, IPerceptionHandler, IActionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Perception? Perception { get => _perception; set => _perception = value; }
        public Action? Action { get => _action; set => _action = value; }
        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        protected FSM? _fsm;
        protected State? _targetState;
        Perception? _perception;
        Action? _action;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public void SetFSM(FSM fsm) => _fsm = fsm;
        public void SetTargetState(State target) => _targetState = target;

        public override void Initialize()
        {
            base.Initialize();
            _fsm = BehaviourGraph as FSM;
            _targetState = TargetNode as State;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public void Start() => Perception?.Start();
        public void Stop() => Perception?.Stop();
        public virtual bool Check() => Perception?.Check() ?? false;
        public virtual void Perform()
        {
            if(Action != null)
            {
                Action.Start();
                Action.Update();
                Action.Stop();
            }
            _fsm?.SetCurrentState(_targetState);
        }

        #endregion
    }
}
