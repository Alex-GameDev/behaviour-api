namespace BehaviourAPI.Core.Perceptions
{
    public abstract class Perception
    {
        public virtual void Start() { }
        public abstract bool Check();
        public virtual void Stop() { }
    }
}
