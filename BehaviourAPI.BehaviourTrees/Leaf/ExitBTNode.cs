namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    public class ExitBTNode : LeafNode
    {
        #region ------------------------------------------- Fields -------------------------------------------

        public Status ReturnedStatus = Status.Sucess;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public ExitBTNode SetReturnedStatus(Status status)
        {
            ReturnedStatus = status;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start() => BehaviourGraph?.Finish(ReturnedStatus);

        // This method should never be executed cause start method will always exit this node.
        protected override Status UpdateStatus()
        {
            return Status.Error;
        }

        #endregion
    }
}
