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

        State? _targetState;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public void SetTargetState(State target) => _targetState = target;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public void Start() => Perception?.Start();
        public void Stop() => Perception?.Stop();
        public virtual bool Check() => Perception?.Check() ?? false;

        public State? GetTargetState() => _targetState;

        #endregion
    }
}
