using BehaviourAPI.Core;
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
       
        public Variable<bool> IsRandomized;

        protected List<BTNode> m_children;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------
        
        public CompositeNode()
        {
            m_children = new List<BTNode>();
        }

        public void AddChild(BTNode child) => m_children.Add(child);

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] is BTNode t)
                    m_children.Add(t);
                else
                    throw new ArgumentException();
            }
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();

            if (m_children.Count == 0)
                throw new MissingChildException(this);

            if (IsRandomized.Value) m_children.OrderBy((guid) => Guid.NewGuid());
        }

        protected BTNode GetBTChildAt(int idx)
        {
            if (m_children.Count == 0)
                throw new MissingChildException(this);

            if (idx < 0 || idx >= m_children.Count) return null;
            return m_children[idx];
        }

        protected int BTChildCount => m_children.Count;

        #endregion
    }
}