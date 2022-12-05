using BehaviourAPI.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.BehaviourTrees
{
    /// <summary>
    /// BTNode type that has multiple children and executes them according to certain conditions.
    /// </summary>
    public abstract class CompositeNode : BTNode
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public sealed override int MaxOutputConnections => -1;
        #endregion

        #region ------------------------------------------- Fields -------------------------------------------
       
        public bool IsRandomized;

        protected List<BTNode> m_children;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------
        
        public CompositeNode()
        {
            m_children = new List<BTNode>();
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();

            m_children = new List<BTNode>();
            for (int i = 0; i < ChildCount; i++)
            {
                if (GetChildAt(i) is BTNode bTNode) 
                    m_children.Add(bTNode);
                else 
                    throw new ArgumentException();
            }

            if (m_children.Count == 0)
                throw new MissingChildException(this);

            if (IsRandomized) m_children.OrderBy((guid) => Guid.NewGuid());
        }

        protected BTNode GetBTChildAt(int idx)
        {
            if (m_children.Count == 0)
                throw new MissingChildException(this);

            if (idx < 0 || idx >= m_children.Count) return null;
            return m_children[idx];
        }

        #endregion
    }
}