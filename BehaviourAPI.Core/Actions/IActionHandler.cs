namespace BehaviourAPI.Core.Actions
{
    using Core;
    /// <summary>
    /// Interface implemented by the nodes and conections that executes an action.
    /// </summary>
    public interface IActionHandler
    {
        public Action? Action { get; set; }
    }
}
