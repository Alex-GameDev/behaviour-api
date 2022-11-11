namespace BehaviourAPI.Core.Perceptions
{
    public class ConditionPerception : Perception
    {
        Func<bool> _onCheck;
        Action? _onInit;
        Action? _onReset;

        public ConditionPerception(Action? onInit, Func<bool> onCheck, Action? onReset = null)
        {
            _onInit = onInit;
            _onCheck = onCheck;
            _onReset = onReset;
        }

        public ConditionPerception(Func<bool> check, Action? stop = null)
        {
            _onCheck = check;
            _onReset = stop;
        }

        public override void Initialize() => _onInit?.Invoke();
        public override bool Check() => _onCheck.Invoke();
        public override void Reset() => _onReset?.Invoke();
    }
}
