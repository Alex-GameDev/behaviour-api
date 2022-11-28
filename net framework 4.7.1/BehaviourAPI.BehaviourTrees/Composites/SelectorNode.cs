namespace BehaviourAPI.BehaviourTrees
{
    using BehaviourAPI.Core;

    /// <summary>
    /// Serial Composite node that executes its children until one of them returns Succeded.
    /// </summary>
    public class SelectorNode : SerialCompositeNode
    {
        public override Status KeepExecutingStatus => Status.Failure;
    }
}