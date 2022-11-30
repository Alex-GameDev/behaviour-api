using System.Timers;

namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Perception that returns false until a determined amount of time.
    /// </summary>
    public class TimerPerception : Perception
    {
        public float Time;

        System.Timers.Timer _timer;

        bool _isTimeout;

        public TimerPerception(float time)
        {
            Time = time;
            _timer = new Timer(time * 1000);
            _timer.Elapsed += OnTimerElapsed;
        }

        public override void Initialize()
        {
            _isTimeout = false;
            _timer.Enabled = true;
            _timer.Start();
        }

        public override bool Check()
        {
            return _isTimeout;
        }

        public override void Reset()
        {
            _isTimeout = false;
            _timer.Enabled = false;
            _timer.Stop();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs evt)
        {
            _isTimeout = true;
        }
    }
}
