namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// A perception that is not tied to an IPerceptionHandler, but is triggered externally.
    /// </summary>
    public class PushPerception
    {
        public IPushActivable PushActivable;

        public PushPerception(IPushActivable pushActivable)
        {
            PushActivable = pushActivable;
        }

        public void Fire() => PushActivable?.Fire();
    }
}
