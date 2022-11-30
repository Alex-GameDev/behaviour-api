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

        List<BTNode> m_children;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------
        
        public CompositeNode()
        {
            m_children = new List<BTNode>();
        }

        public void AddChild(BTNode child) => m_children.Add(child);

        public override void Initialize()
        {
            base.Initialize();
            Children.ForEach(node => m_children.Add(node as BTNode));
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            if (IsRandomized) m_children.OrderBy((guid) => Guid.NewGuid());
        }

        protected BTNode GetChildAt(int idx)
        {
            if (idx < 0 || idx >= m_children.Count) return null;
            return m_children[idx];
        }

        protected int ChildCount => m_children.Count;
        protected List<BTNode> GetChildren() => m_children;

        #endregion
    }
}