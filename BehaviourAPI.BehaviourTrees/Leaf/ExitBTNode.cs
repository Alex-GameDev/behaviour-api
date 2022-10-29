namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    public class ExitBTNode : LeafNode
    {
        Status _returnedStatus = Status.Sucess;

        public ExitBTNode SetReturnedStatus(Status status)
        {
            _returnedStatus = status;
            return this;
        }

        public override void Start() => BehaviourGraph?.Finish(_returnedStatus);

        // This method should never be executed cause start method will always exit this node.
        protected override Status UpdateStatus()
        {
            return Status.Error;
        }
    }
}
