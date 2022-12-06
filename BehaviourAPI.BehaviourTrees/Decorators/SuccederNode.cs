namespace BehaviourAPI.BehaviourTrees
{
    using BehaviourAPI.BehaviourTrees.Decorators;
    using BehaviourAPI.Core.Exceptions;
    using Core;
    /// <summary>
    /// Node that changes the result returned by its child node to Succeded if it's Failure.
    /// </summary>
    public class SuccederNode : DirectDecoratorNode
    {
        #region --------------------------------------- Runtime methods --------------------------------------

        protected override Status UpdateStatus()
        {
            if (m_childNode == null)
                throw new MissingChildException(this);

            m_childNode.Update();
            var status = m_childNode.Status;
            if (status == Status.Failure) status = Status.Success;
            return status;
        }
        #endregion
    }
}