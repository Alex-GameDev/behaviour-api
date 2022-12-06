namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    using System;
    using System.Collections.Generic;

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

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            if (children.Count > 0 && children[0] is BTNode bTNode)
                m_childNode = bTNode;
            else
                throw new ArgumentException();
        }

        #endregion
    }
}