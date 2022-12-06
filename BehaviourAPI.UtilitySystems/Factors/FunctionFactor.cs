namespace BehaviourAPI.UtilitySystems
{
    using Core;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Factor that modifies its child value with a function.
    /// </summary>  
    public abstract class FunctionFactor : Factor
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override int MaxOutputConnections => 1;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        Factor m_childFactor;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public FunctionFactor SetChild(Factor factor)
        {
            m_childFactor = factor;
            return this;
        }

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            if (children.Count > 0 && children[0] is Factor factor)
                m_childFactor = factor;
            else
                throw new ArgumentException();
        }



        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        protected override float ComputeUtility()
        {
            m_childFactor?.UpdateUtility();
            return Evaluate(m_childFactor?.Utility ?? 0f);
        }

        protected abstract float Evaluate(float childUtility);
        #endregion
    }
}
