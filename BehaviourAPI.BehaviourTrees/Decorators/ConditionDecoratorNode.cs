using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.BehaviourTrees
{
    /// <summary>
    /// Decorator that executes its child only if a perception is checked. Starts or stops the child execution 
    /// when the perception value changes.
    /// </summary>
    public class ConditionDecoratorNode : DecoratorNode, IPerceptionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Perception? Perception { get => _perception; set => _perception = value; }
        Perception? _perception;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        bool _childExecutedLastFrame;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public ConditionDecoratorNode SetPerception(Perception perception)
        {
            _perception = perception;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();

            if(Perception != null)
            {
                Perception.Start();
            }
            else
                throw new NullReferenceException("ERROR: Perception is not defined.");
        }

        public override void Stop()
        {
            base.Stop();

            if (Perception != null)
            {
                Perception.Stop();
            }
            else
                throw new NullReferenceException("ERROR: Perception is not defined.");
        }

        protected override Status UpdateStatus()
        {
            if (Perception != null)
            {
                if(m_childNode != null)
                {
                    if(Perception.Check())
                    {
                        if (!_childExecutedLastFrame) m_childNode.Start();
                        _childExecutedLastFrame = true;
                        m_childNode.Update();
                        return m_childNode.Status;
                    }
                    else
                    {
                        if (_childExecutedLastFrame) m_childNode.Stop();
                        _childExecutedLastFrame = false;
                        return Status.Running;
                    }
                }
                throw new NullReferenceException("ERROR: Child node is not defined.");
            }
            throw new NullReferenceException("ERROR: Perception is not defined.");
        }

        #endregion
    }
}
