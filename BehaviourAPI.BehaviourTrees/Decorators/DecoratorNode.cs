namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    using System;

    /// <summary>
    /// BTNode that alters the result returned by its child node or its execution.
    /// </summary>
    public abstract class DecoratorNode : BTNode
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public sealed override int MaxOutputConnections => 1;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        protected BTNode m_childNode;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------
        public void SetChild(BTNode child) => m_childNode = child;

        public override void Initialize()
        {
            base.Initialize();
            if (Children.Count == 1)
            {
                m_childNode = GetFirstChild() as BTNode;
            }
            else
            {
                throw new Exception();
            }
        }
        
        #endregion
    }
}