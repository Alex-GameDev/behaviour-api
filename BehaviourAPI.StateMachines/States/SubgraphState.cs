namespace BehaviourAPI.StateMachines
{
    using Core;

    /// <summary>
    /// A State node that executes an Action.
    /// </summary>
    public abstract class SubgraphState : State, ISubgraphHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override string Description => "State Node that executes a subgraph";
        public behaviourGraph? Subgraph { get; set; }

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public SubgraphState SetSubgraph(behaviourGraph behaviourEngine)
        {
            Subgraph = behaviourEngine;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            Subgraph?.Start();
        }

        protected override Status UpdateStatus()
        {
            Subgraph?.Update();
            return Subgraph?.Status ?? Status.Error;
        }

        public override void Stop()
        {
            base.Stop();
            Subgraph?.Stop();
        }

        #endregion
    }
}
