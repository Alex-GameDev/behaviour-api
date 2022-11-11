using System.Timers;

namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Perception that
    /// </summary>
    public class TimerPerception : Perception
    {
        public float Time;

        System.Timers.Timer _timer;

        bool _isTimeout;
        bool _launched;

        public TimerPerception(float time)
        {
            Time = time;
            _timer = new System.Timers.Timer(time * 1000);
            _timer.Elapsed += OnTimerElapsed;
        }        

        public override bool Check()
        {
            if(_launched)
            {
                _isTimeout = false;
                _timer.Enabled = true;
                _timer.Start();
            }
            return _isTimeout;
        }

        public override void Reset()
        {
            _isTimeout = false;
            _timer.Enabled = false;
            _timer.Stop();
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs evt)
        {
            _isTimeout = true;
        }
    }
}
