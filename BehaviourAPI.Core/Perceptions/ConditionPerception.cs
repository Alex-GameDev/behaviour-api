namespace BehaviourAPI.Core.Perceptions
{
    public class ConditionPerception : Perception
    {
        Func<bool> _check;
        Action? _start;
        Action? _stop;

        public ConditionPerception(Action? start, Func<bool> update, Action? stop = null)
        {
            _start = start;
            _check = update;
            _stop = stop;
        }

        public ConditionPerception(Func<bool> check, Action? stop = null)
        {
            _check = check;
            _stop = stop;
        }

        public override void Start() => _start?.Invoke();
        public override bool Check() => _check.Invoke();
        public override void Stop() => _stop?.Invoke();
    }
}
