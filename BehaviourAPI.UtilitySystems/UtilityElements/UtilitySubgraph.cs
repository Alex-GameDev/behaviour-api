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
 
        public BehaviourGraph? Subgraph { get => _subgraph; set => _subgraph = value; }

        BehaviourGraph? _subgraph;

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
        }

        public override void Stop()
        {
            Subgraph?.Stop();
        }

        #endregion
    }
}
