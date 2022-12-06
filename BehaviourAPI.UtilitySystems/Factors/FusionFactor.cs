using BehaviourAPI.Core;
using System;
using System.Collections.Generic;

namespace BehaviourAPI.UtilitySystems
{
    public abstract class FusionFactor : Factor
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override int MaxOutputConnections => -1;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        protected List<Factor> m_childFactors;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public FusionFactor()
        {
            m_childFactors = new List<Factor>();
        }

        public void AddFactor(Factor factor) => m_childFactors.Add(factor);

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] is Factor f)
                    m_childFactors.Add(f);
                else
                    throw new ArgumentException();
            }
        }


        #endregion
    }
}
