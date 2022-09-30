namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using Core;
    /// <summary>
    /// BTNode that alters the result returned by its child node or its execution.
    /// </summary>
    public abstract class DecoratorNode : BTNode
    {
        public sealed override int MaxOutputConnections => 1;
        BTNode m_childNode;
        bool m_resetChildFlag;

        public override void Start() => m_childNode.Start();

        public override Status UpdateStatus()
        {
            if (m_resetChildFlag)
            {
                m_childNode.Start();
                m_resetChildFlag = false;
            }
            m_childNode.Update();
            var status = m_childNode.Status;
            return GetModifiedChildStatus(status);
        }

        protected abstract Status GetModifiedChildStatus(Status childStatus);

        protected void ResetChild() => m_resetChildFlag = true;
    }
}