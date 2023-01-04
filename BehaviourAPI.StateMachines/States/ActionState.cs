namespace BehaviourAPI.StateMachines
{
    using Core;
    using Core.Actions;
    using System;
    using System.Collections.Generic;
    using Action = Core.Actions.Action;

    public class ActionState : State
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override int MaxOutputConnections => -1;

        #endregion

        #region -------------------------------------------- Fields ------------------------------------------

        public Action Action;

        protected List<Transition> _transitions;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public ActionState()
        {
            _transitions = new List<Transition>();
        }

        public void AddTransition(Transition transition) => _transitions.Add(transition);

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            for(int i = 0; i < children.Count; i++)
            {
                if (children[i] is Transition t)
                    _transitions.Add(t);
                else
                    throw new ArgumentException();
            }
        }

        public ActionState SetAction(Action action)
        {
            Action = action;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            Status = Status.Running;
            _transitions.ForEach(t => t?.Start());
            Action?.Start();
        }

        public override void Update()
        {
            Status = Action?.Update() ?? Status.Running;
            CheckTransitions();           
        }

        public override void Stop()
        {
            Status = Status.None;
            _transitions.ForEach(t => t?.Stop());
            Action?.Stop();
        }

        protected virtual bool CheckTransitions()
        {
            for(int i = 0; i < _transitions.Count; i++)
            {
                if(_transitions[i].isPulled && _transitions[i].Check())
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
