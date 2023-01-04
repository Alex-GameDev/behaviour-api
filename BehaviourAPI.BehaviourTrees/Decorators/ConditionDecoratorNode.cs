using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.BehaviourTrees.Decorators
{
    using BehaviourAPI.Core.Exceptions;
    using Core.Perceptions;

    /// <summary>
    /// Decorator that executes its child only if a perception is triggered. Perception is checked at the start
    /// and return Failure if isn't triggered. Otherwise execute the child and returns its value.
    /// </summary>
    public class ConditionDecoratorNode : DecoratorNode
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Perception Perception;

        bool _executeChild;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public ConditionDecoratorNode SetPerception(Perception perception)
        {
            Perception = perception;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            if (Perception != null)
            {
                Perception.Initialize();
                _executeChild = Perception.Check();
                Perception.Reset();                
            }
            else
                throw new NullReferenceException("ERROR: Perception is not defined.");

            if (_executeChild)
            {
                if (m_childNode == null)
                    throw new MissingChildException(this);

                m_childNode.Start();
            }
        }

        public override void Stop()
        {
            base.Stop();
            if(_executeChild)
            {
                if (m_childNode == null)
                    throw new MissingChildException(this);

                m_childNode.Stop();
            }            
        }

        protected override Status UpdateStatus()
        {
            if (_executeChild)
            {
                if (m_childNode == null)
                    throw new MissingChildException(this);

                m_childNode.Update();
                return m_childNode.Status;
            }
            else
            {                    
                return Status.Failure;
            }
        }

        #endregion
    }
}
