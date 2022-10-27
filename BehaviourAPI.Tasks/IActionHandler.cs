namespace BehaviourAPI.Core
{
    /// <summary>
    /// Interface implemented by the nodes and conections that executes an action.
    /// </summary>
    public interface IActionHandler
    {
        public Func<ExecutionPhase, Status>? Action { get; set; }
    }
}
