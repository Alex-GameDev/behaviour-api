namespace BehaviourAPI.BehaviourTrees
{
    using BehaviourAPI.Core;

    /// <summary>
    /// Serial Composite node that executes its children until one of them returns Failure.
    /// </summary>
    public class SequencerNode : SerialCompositeNode
    {
        public override string Description => "Composite node that executes its childs one by one until some of them returns Failure";
        public override Status KeepExecutingStatus => Status.Success;
    }
}