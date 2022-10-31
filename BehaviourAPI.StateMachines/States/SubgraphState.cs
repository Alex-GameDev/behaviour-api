namespace BehaviourAPI.StateMachines
{
    using Core;

    /// <summary>
    /// A State node that executes an Subgraph.
    /// </summary>
    public class SubgraphState : State, ISubgraphHandler
    {

        #region ------------------------------------------ Properties -----------------------------------------

        public override string Description => "State Node that executes a _subgraph";
        public BehaviourGraph? Subgraph { get => _subgraph; set => _subgraph = value; }
        BehaviourGraph? _subgraph;
        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public SubgraphState SetSubgraph(BehaviourGraph behaviourEngine)
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
