namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    public class PerceptionBTNode : LeafNode, IPerceptionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override string Description => "Behaviour Tree Node that executes a perception";
        public Func<ExecutionPhase, bool>? Perception { get; set; }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            Perception?.Invoke(ExecutionPhase.START);
        }

        protected override Status UpdateStatus()
        {
            bool check = Perception?.Invoke(ExecutionPhase.UPDATE) ?? false;
            return check.ToStatus();
        }
        public override void Stop()
        {
            base.Stop();
            Perception?.Invoke(ExecutionPhase.STOP);
        }

        #endregion
    }
}
