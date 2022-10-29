namespace BehaviourAPI.BehaviourTrees.Leaf
{
    using Core;
    public class SubgraphNode : LeafNode, ISubgraphHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override string Description => "Behaviour Tree Node that executes a subgraph";
        public BehaviourGraph? Subgraph { get; set; }

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public SubgraphNode SetSubgraph(BehaviourGraph behaviourEngine)
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
