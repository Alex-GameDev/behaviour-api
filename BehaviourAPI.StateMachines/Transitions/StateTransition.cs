using BehaviourAPI.Core.Exceptions;

namespace BehaviourAPI.StateMachines
{
    public class StateTransition : Transition
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override int MaxOutputConnections => 1;

        protected State _targetState;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public void SetTargetState(State target) => _targetState = target;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Perform()
        {
            base.Perform();
            if (_targetState == null)
                throw new MissingChildException(this, "The target status can't be null.");

            _fsm.SetCurrentState(_targetState);
        }

        #endregion
    }
}
