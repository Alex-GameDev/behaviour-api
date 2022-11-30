namespace BehaviourAPI.StateMachines
{
    using BehaviourAPI.Core.Actions;
    using BehaviourAPI.Core.Perceptions;
    using System;
    using Action = Core.Actions.Action;

    public class Transition : FSMNode, IPerceptionHandler, IActionHandler, IPushActivable
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Perception Perception { get; set; }
        public Action Action { get; set; }

        public override Type ChildType => typeof(State);

        public override int MaxInputConnections => 1;

        public override int MaxOutputConnections => 1;
        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        protected FSM _fsm;
        protected State _sourceState;
        protected State _targetState;

        public bool isPulled = true;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public void SetFSM(FSM fsm) => _fsm = fsm;
        public void SetSourceState(State source) => _sourceState = source;
        public void SetTargetState(State target) => _targetState = target;

        public override void Initialize()
        {
            base.Initialize();
            _fsm = BehaviourGraph as FSM;
            _targetState = GetFirstChild() as State;
            _sourceState = GetFirstParent() as State;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public void Start() => Perception?.Initialize();
        public void Stop() => Perception?.Reset();
        public virtual bool Check() => Perception?.Check() ?? true;
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
