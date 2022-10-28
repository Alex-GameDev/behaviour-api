namespace BehaviourAPI.BehaviourTrees
{
    using BehaviourAPI.Core.Perceptions;
    using Core;
    public class PerceptionBTNode : LeafNode, IPerceptionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override string Description => "Behaviour Tree Node that executes a perception";
        public Perception? Perception { get; set; }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            Perception?.Start();
        }

        protected override Status UpdateStatus()
        {
            bool check = Perception?.Check() ?? false;
            return check.ToStatus();
        }
        public override void Stop()
        {
            base.Stop();
            Perception?.Stop();
        }

        #endregion
    }
}
