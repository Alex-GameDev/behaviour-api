﻿namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    using Core.Perceptions;

    /// <summary>
    /// Decorator that executes its child only if a perception is triggered. Perception is checked every frame
    /// and starts or stops the child execution when the value changes.
    /// </summary>
    public class SwitchDecoratorNode : DecoratorNode, IPerceptionHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public Perception? Perception { get => _perception; set => _perception = value; }
        Perception? _perception;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        bool _childExecutedLastFrame;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public SwitchDecoratorNode SetPerception(Perception perception)
        {
            _perception = perception;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            if(Perception != null) Perception.Initialize();
            else  throw new NullReferenceException("ERROR: Perception is not defined.");
        }

        public override void Stop()
        {
            base.Stop();
            if(m_childNode != null && _childExecutedLastFrame) m_childNode.Stop();
            if (Perception != null) Perception.Reset();
            else throw new NullReferenceException("ERROR: Perception is not defined.");
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