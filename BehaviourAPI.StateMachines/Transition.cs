namespace BehaviourAPI.StateMachines
{
    using Core;

    public class Transition : Connection, IPerceptionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Func<ExecutionPhase, bool>? Perception { get; set; }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        State? _targetState;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public void SetTargetState(State target) => _targetState = target;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public void Start() => Perception?.Invoke(ExecutionPhase.START);
        public void Stop() => Perception?.Invoke(ExecutionPhase.STOP);

        public bool Check() => Perception?.Invoke(ExecutionPhase.UPDATE) ?? false;

        public State? GetTargetState() => _targetState;

        #endregion
    }
}
