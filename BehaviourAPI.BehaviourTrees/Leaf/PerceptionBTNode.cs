namespace BehaviourAPI.BehaviourTrees
{
    using BehaviourAPI.Core.Perceptions;
    using Core;
    public class PerceptionBTNode : LeafNode, IPerceptionHandler
    {

        #region ------------------------------------------ Properties -----------------------------------------

        public override string Description => "Behaviour Tree Node that executes a _perception";
        public Perception? Perception { get => _perception; set => _perception = value; }
        Perception? _perception;
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
