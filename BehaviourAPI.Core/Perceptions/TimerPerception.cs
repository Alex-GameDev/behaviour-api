using System.Timers;

namespace BehaviourAPI.Core.Perceptions
{

    public class TimerPerception : Perception
    {
        public float Time;

        System.Timers.Timer _timer;

        bool _isTimeout;

        public TimerPerception(float time)
        {
            Time = time;
            _timer = new System.Timers.Timer(time * 1000);
            _timer.Elapsed += OnTimerElapsed;
        }

        public override bool Check()
        {
            return _isTimeout;
        }

        public override void Start()
        {
            base.Start();
            _isTimeout = false;
            _timer.Enabled = true;
            _timer.Start();
        }

        public override void Stop()
        {
            base.Stop();
            _isTimeout = false;
            _timer.Enabled= false;
            _timer.Stop();
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs evt)
        {
            _isTimeout = true;
        }
    }
}
