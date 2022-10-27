namespace BehaviourAPI.Tasks
{
    using Core;
    /// <summary>
    /// Interface implemented by the nodes and conections that executes a perception.
    /// </summary>
    public interface IPerceptionHandler
    {
        public Func<ExecutionPhase, bool>? Perception { get; set; }
    }
}
