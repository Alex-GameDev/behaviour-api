namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    /// <summary>
    /// Node that execute its child node until returns a given value.
    /// </summary>
    public class LoopUntilNode : DecoratorNode
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override string Description => "Decorator node that executes its child until it returns the desired value";

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        public Status TargetStatus;
        public int MaxIterations = -1;

        int currentIterations;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            currentIterations = 0;
            m_childNode?.Start();
        }

        protected override Status UpdateStatus()
        {
            m_childNode?.Update();
            var status = m_childNode?.Status ?? Status.Error;
            if (status == TargetStatus.Inverted())
            {
                currentIterations++;
                if(currentIterations != MaxIterations)
                {
                    // Restart the node execution
                    status = Status.Running;
                    m_childNode?.Stop();
                    m_childNode?.Start();
                }                
            }
            return status;
        }
        #endregion

    }
}