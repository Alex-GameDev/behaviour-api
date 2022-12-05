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

        public override void Start()
        {
            base.Start();

            if (GetFirstChild() is BTNode bTNode)
                m_childNode = bTNode;
            else
                throw new ArgumentException();
        }
    }
}