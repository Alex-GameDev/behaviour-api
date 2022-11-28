using BehaviourAPI.Core.Actions;

namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Interface implemented by the nodes and conections that executes a perception.
    /// </summary>
    public interface IPerceptionHandler
    {
        public Perception? Perception { get; set; }
    }
}
