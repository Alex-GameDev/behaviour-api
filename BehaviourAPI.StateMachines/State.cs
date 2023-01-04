using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using System;
using System.Collections.Generic;
using Action = BehaviourAPI.Core.Actions.Action;

namespace BehaviourAPI.StateMachines
{
    public class State : FSMNode, IStatusHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override Type ChildType => typeof(Transition);

        public override int MaxInputConnections => -1;
        public override int MaxOutputConnections => -1;

        public Status Status
        {
            get => _status;
            protected set
            {
                if (_status != value)
                {
                    _status = value;
                    StatusChanged?.Invoke(_status);
                }
            }
        }

        public Action<Status> StatusChanged { get; set; }

        Status _status;

        #endregion

        #region -------------------------------------------- Fields ------------------------------------------

        public Action Action;

        protected List<Transition> _transitions;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public State()
        {
            _transitions = new List<Transition>();
        }

        public void AddTransition(Transition transition) => _transitions.Add(transition);

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] is Transition t)
                    _transitions.Add(t);
                else
                    throw new ArgumentException();
            }
        }

        public State SetAction(Action action)
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

        public virtual void Update()
        {
            Status = Action?.Update() ?? Status.Running;
            CheckTransitions();
        }

        public virtual void Stop()
        {
            Status = Status.None;
            _transitions.ForEach(t => t?.Stop());
            Action?.Stop();
        }

        protected virtual bool CheckTransitions()
        {
            for (int i = 0; i < _transitions.Count; i++)
            {
                if (_transitions[i].isPulled && _transitions[i].Check())
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
