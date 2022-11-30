namespace BehaviourAPI.Core.Actions
{
    using Core;
    /// <summary>
    /// Interface implemented by the nodes and conections that executes an action.
    /// </summary>
    public interface IActionHandler
    {
        Action Action { get; set; }
    }
}
