using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.BehaviourTrees.Decorators
{
    using Core.Perceptions;

    /// <summary>
    /// Decorator that executes its child only if a perception is triggered. Perception is checked at the start
    /// and return Failure if isn't triggered. Otherwise execute the child and returns its value.
    /// </summary>
    public class ConditionDecoratorNode : DecoratorNode, IPerceptionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Perception? Perception { get; set; }

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
                if (m_childNode != null) m_childNode.Start();
                else throw new NullReferenceException("ERROR: Child node is not defined");
            }
        }

        public override void Stop()
        {
            base.Stop();
            if(_executeChild)
            {
                if (m_childNode != null) m_childNode.Stop();
                else throw new NullReferenceException("ERROR: Child node is not defined");
                _executeChild = false;
            }            
        }

        protected override Status UpdateStatus()
        {
            if (m_childNode != null)
            {
                if (_executeChild)
                {                    
                    m_childNode.Update();
                    return m_childNode.Status;
                }
                else
                {                    
                    return Status.Failure;
                }
            }
            throw new NullReferenceException("ERROR: Child node is not defined.");
        }

        #endregion
    }
}
