namespace BehaviourAPI.Core.Actions
{
    public abstract class Action
    {
        public abstract void Start();
        public abstract Status Update();
        public abstract void Stop();
    }
}
