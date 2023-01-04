namespace BehaviourAPI.StateMachines
{
    using BehaviourAPI.Core;
    using BehaviourAPI.Core.Actions;
    using BehaviourAPI.Core.Exceptions;
    using BehaviourAPI.Core.Perceptions;
    using System;
    using System.Collections.Generic;
    using Action = Core.Actions.Action;

    public abstract class Transition : FSMNode, IPushActivable
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Perception Perception;
        public Action Action;

        public override Type ChildType => typeof(State);

        public override int MaxInputConnections => 1;

        public System.Action TransitionTriggered { get; set; }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        protected FSM _fsm;

        protected State _sourceState;

        public bool isPulled = true;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public void SetFSM(FSM fsm) => _fsm = fsm;
        public void SetSourceState(State source) => _sourceState = source;

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            _fsm = BehaviourGraph as FSM;

            //if (children.Count > 0 && children[0] is State to)
            //    _targetState = to;
            //else
            //    throw new ArgumentException();

            if (parents.Count > 0 && children[0] is State from)
                _sourceState = from;
            else
                throw new ArgumentException();
        }


        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public void Start() => Perception?.Initialize();
        public void Stop() => Perception?.Reset();
        public virtual bool Check() => Perception?.Check() ?? true;
        public virtual void Perform()
        {
            if (!_fsm.IsCurrentState(_sourceState)) return;

            if (Action != null)
            {
                Action.Start();
                Action.Update();
                Action.Stop();
            }

            _fsm.OnTriggerTransition(this);

            TransitionTriggered?.Invoke();
        }

        public void Fire() => Perform();

        #endregion
    }
}
