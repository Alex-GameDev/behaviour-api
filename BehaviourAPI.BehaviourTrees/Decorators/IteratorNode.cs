using BehaviourAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.BehaviourTrees
{
    /// <summary>
    /// Node that execute its child node the number of times determined by <see cref="Itera"/>
    /// </summary>
    public  class IteratorNode : DecoratorNode
    {
        #region ------------------------------------------- Fields -------------------------------------------

        public int Iterations = 1;

        int _currentIterations;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public IteratorNode SetIterations(int iterations)
        {
            Iterations = iterations;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            m_childNode?.Start();
        }

        protected override Status UpdateStatus()
        {
            if(m_childNode != null)
            {
                m_childNode.Update();
                var status = m_childNode?.Status ?? Status.Error;

                // If child execution ends, restart until currentIterations > Iterations
                if(status != Status.Running)
                {
                    _currentIterations++;
                    if(_currentIterations < Iterations)
                    {
                        status = Status.Running;
                        m_childNode?.Stop();
                        m_childNode?.Start();
                    }
                }
                return status;
            }
            throw new NullReferenceException("ERROR: Child node is not defined.");
        }
        #endregion
    }
}
