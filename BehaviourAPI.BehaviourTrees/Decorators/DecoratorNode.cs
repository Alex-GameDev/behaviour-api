namespace BehaviourAPI.BehaviourTrees
{
    using Core;

    /// <summary>
    /// BTNode that alters the result returned by its child node or its execution.
    /// </summary>
    public abstract class DecoratorNode : BTNode
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public sealed override int MaxOutputConnections => 1;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        protected BTNode? m_childNode;
        protected bool m_resetChildFlag;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------
        public void SetChild(BTNode child) => m_childNode = child;

        public override void Initialize()
        {
            base.Initialize();
            if (OutputConnections.Count == 1)
            {
                m_childNode = GetChildNodes().First() as BTNode;
            }
        }
        #endregion
    }
}