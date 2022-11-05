namespace BehaviourAPI.Core.Actions
{
    public class ExitGraphAction : Action
    {
        public Status ReturnedStatus;
        
        public BehaviourGraph? Graph;

        // Graph arg must be the IActionHandler's graph.
        public ExitGraphAction(BehaviourGraph graph, Status returnedStatus = Status.Sucess)
        {
            ReturnedStatus = returnedStatus;
            Graph = graph;
        }

        public override void Start()
        {
            Graph?.Finish(ReturnedStatus);
        }

        // This method should never be executed cause Start method will always exit this node.
        public override Status Update()
        {
            return Status.Error;
        }

        // This method should never be executed cause Start method will always exit this node.
        public override void Stop()
        {           
        }
    }
}
