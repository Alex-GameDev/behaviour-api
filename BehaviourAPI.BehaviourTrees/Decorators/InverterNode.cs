namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    /// <summary>
    /// Node that inverts the result returned by its child node (Success/Failure).
    /// </summary>

    public class InverterNode : DecoratorNode
    {
        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            m_childNode?.Start();
        }

        public override void Stop()
        {
            base.Stop();
            m_childNode?.Stop();
        }

        protected override Status UpdateStatus()
        {
            m_childNode?.Update();
            var status = m_childNode?.Status ?? Status.Error;            
            return status.Inverted();
        }
        #endregion
    }
}