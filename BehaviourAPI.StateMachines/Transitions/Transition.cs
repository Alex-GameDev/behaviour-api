namespace BehaviourAPI.StateMachines
{
    using BehaviourAPI.Core.Perceptions;
    using Core;

    public class Transition : Connection, IPerceptionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Perception? Perception { get; set; }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        protected FSM? _fsm;
        protected State? _targetState;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        internal void SetFSM(FSM fsm) => _fsm = fsm;
        internal void SetTargetState(State target) => _targetState = target;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public void Start() => Perception?.Start();
        public void Stop() => Perception?.Stop();
        public virtual bool Check() => Perception?.Check() ?? false;

        public virtual void Perform() => _fsm?.SetCurrentState(_targetState);

        #endregion
    }
}
