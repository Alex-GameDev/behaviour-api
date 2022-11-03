namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    /// <summary>
    /// Node that changes the result returned by its child node to Succeded if it's Failure.
    /// </summary>
    public class SuccederNode : DecoratorNode
    {
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
            if (status == Status.Failure) status = Status.Sucess;
            return status;
        }
        #endregion
    }
}