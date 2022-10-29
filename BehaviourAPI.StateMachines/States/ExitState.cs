namespace BehaviourAPI.StateMachines
{
    using Core;
    public class ExitState : State
    {
        Status _returnedStatus = Status.Sucess;

        public ExitState SetReturnedStatus(Status status)
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
