using BehaviourAPI.BehaviourTrees.Decorators;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Exceptions;
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
    public  class IteratorNode : DirectDecoratorNode
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
            _currentIterations = 0;
        }

        protected override Status UpdateStatus()
        {
            if (m_childNode == null)
                throw new MissingChildException(this);

                m_childNode.Update();
                var status = m_childNode.Status;

            // If child execution ends, restart until currentIterations > Iterations
            if(status != Status.Running)
            {
                _currentIterations++;
                if(Iterations == -1 || _currentIterations < Iterations)
                {
                    status = Status.Running;
                    m_childNode.Stop();
                    m_childNode.Start();
                }
            }
            return status;
        }
        #endregion
    }
}
