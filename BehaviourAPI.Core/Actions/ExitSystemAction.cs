namespace BehaviourAPI.Core.Actions
{
    public class ExitSystemAction : Action
    {
        public Status ReturnedStatus;
        
        public BehaviourSystem Graph;

        // Graph arg must be the IActionHandler's graph.
        public ExitSystemAction(BehaviourSystem graph, Status returnedStatus = Status.Success)
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
