namespace BehaviourAPI.BehaviourTrees
{
    using BehaviourAPI.BehaviourTrees.Decorators;
    using BehaviourAPI.Core.Exceptions;
    using Core;
    /// <summary>
    /// Node that inverts the result returned by its child node (Success/Failure).
    /// </summary>

    public class InverterNode : DirectDecoratorNode
    {
        public override string Description => "Decorator node that return the status of its child inverted";

        #region --------------------------------------- Runtime methods --------------------------------------

        protected override Status UpdateStatus()
        {
            if (m_childNode == null)
                throw new MissingChildException(this);

            m_childNode.Update();
            var status = m_childNode.Status;            
            return status.Inverted();
        }
        #endregion
    }
}