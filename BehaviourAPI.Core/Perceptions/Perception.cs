namespace BehaviourAPI.Core.Perceptions
{
    public abstract class Perception
    {
        public virtual void Initialize() { }
        public abstract bool Check();
        public virtual void Reset() { }
    }
}
