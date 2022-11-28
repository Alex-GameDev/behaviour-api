namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// A perception that is not tied to an IPerceptionHandler, but is triggered externally.
    /// </summary>
    public class PushPerception
    {
        public List<IPushActivable> PushListeners;

        public PushPerception(params IPushActivable[] listeners)
        {
            PushListeners = listeners.ToList();
        }
        public PushPerception(List<IPushActivable> listeners)
        {
            PushListeners = listeners.ToList();
        }

        public void Fire() => PushListeners.ForEach(p => p?.Fire());
    }
}
