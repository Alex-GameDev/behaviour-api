namespace BehaviourAPI.UtilitySystems
{
    using Core;

    /// <summary>
    /// Utility element that handle multiple <see cref="UtilitySelectableNode"/> itself and
    /// returns the maximum utility if its best candidate utility is higher than the threshold.
    /// </summary>
    public class UtilitySubgraph : UtilityElement, ISubgraphHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override string Description => "Utility element that executes a subgraph.";
        public BehaviourGraph? Subgraph { get; set; }

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public UtilitySubgraph SetSubgraph(BehaviourGraph behaviourEngine)
        {
            Subgraph = behaviourEngine;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------
        public override void Start()
        {
            Subgraph?.Start();
        }

        protected override Status UpdateStatus()
        {
            Subgraph?.Update();
            return Subgraph?.Status ?? Status.Error;
            //Restart
        }

        public override void Stop()
        {
            Subgraph?.Stop();
        }

        #endregion
    }
}
