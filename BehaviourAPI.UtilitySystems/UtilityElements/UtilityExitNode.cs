namespace BehaviourAPI.UtilitySystems
{
    using Core;
    public class UtilityExitNode : UtilityElement
    {
        Status _returnedStatus = Status.Sucess;

        public UtilityExitNode SetReturnedStatus(Status status)
        {
            _returnedStatus = status;
            return this;
        }

        public override void Start() => BehaviourGraph?.Finish(_returnedStatus);

        // This methods should never be executed cause start method will always exit this node.
        protected override Status UpdateStatus()
        {
            return Status.Error;
        }

        public override void Stop()
        {
        }
    }
}
