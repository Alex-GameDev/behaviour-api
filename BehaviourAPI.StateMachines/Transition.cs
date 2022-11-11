﻿namespace BehaviourAPI.StateMachines
{
    using BehaviourAPI.Core.Actions;
    using BehaviourAPI.Core.Perceptions;
    using Core;

    public class Transition : Connection, IPerceptionHandler, IActionHandler, IPushActivable
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Perception? Perception { get => _perception; set => _perception = value; }
        public Action? Action { get => _action; set => _action = value; }
        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        protected FSM? _fsm;
        protected State? _sourceState;
        protected State? _targetState;
        Perception? _perception;
        Action? _action;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public void SetFSM(FSM fsm) => _fsm = fsm;
        public void SetSourceState(State source) => _sourceState = source;
        public void SetTargetState(State target) => _targetState = target;

        public override void Initialize()
        {
            base.Initialize();
            _fsm = BehaviourGraph as FSM;
            _targetState = TargetNode as State;
            _targetState = SourceNode as State;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public void Start() => Perception?.Initialize();
        public void Stop() => Perception?.Reset();
        public virtual bool Check() => Perception?.Check() ?? false;
        public virtual void Perform()
        {
            if (!(_fsm?.IsCurrentState(_sourceState) ?? false)) return;

            if (Action != null)
            {
                Action.Start();
                Action.Update();
                Action.Stop();
            }
            _fsm?.OnTriggerTransition(this);
            _fsm?.SetCurrentState(_targetState);
        }

        public void Fire() => Perform();

        #endregion
    }
}
