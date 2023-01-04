using System.Timers;

namespace BehaviourAPI.BehaviourTrees
{
    using BehaviourAPI.Core.Exceptions;
    using Core;
    using System;

    /// <summary>
    /// Decorator that waits an amount of time to execute the child.
    /// </summary>
    public class TimerDecoratorNode : DecoratorNode
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override string Description => "Decorator node which waits a certain time to execute its child";

        public float Time;
        Timer _timer;

        bool _isTimeout;
        bool _childExecuted;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public TimerDecoratorNode SetTime(float time)
        {
            Time = time;
            return this;
        }

        public override void Start()
        {
            base.Start();
            _childExecuted = false;
            _timer = new Timer(Time * 1000);
            _timer.Elapsed += OnTimerElapsed;

            _isTimeout = false;
            _timer.Enabled = true;
            _timer.Start();
        }

        #endregion
        protected override Status UpdateStatus()
        {
            if (!_isTimeout) return Status.Running;
            
            if (m_childNode != null)
            {
                if (!_childExecuted)
                {
                    m_childNode.Start();
                    _childExecuted = true;
                }
                m_childNode.Update();
                return m_childNode.Status;
            }
            throw new MissingChildException(this);
        }

        public override void Stop()
        {
            base.Stop();
            _isTimeout = false;
            if(_timer != null)
            { 
                _timer.Enabled = false;
                _timer.Stop();
            }

            if(_childExecuted)
            {
                if (m_childNode == null)
                    throw new MissingChildException(this);

                m_childNode.Stop();
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs evt)
        {
            _isTimeout = true;
        }
    }
}
