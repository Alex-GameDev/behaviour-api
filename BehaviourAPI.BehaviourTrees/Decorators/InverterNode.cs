namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    /// <summary>
    /// Node that inverts the result returned by its child node (Success/Failure).
    /// </summary>

    public class InverterNode : DecoratorNode
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override string Description => "Decorator that returns the inverted status of its child.";

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            m_childNode?.Start();
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