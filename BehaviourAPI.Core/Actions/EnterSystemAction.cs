namespace BehaviourAPI.Core.Actions
{
    public class EnterSystemAction : Action
    {
        public BehaviourSystem? Subgraph;

        public EnterSystemAction(BehaviourSystem? subgraph)
        {
            Subgraph = subgraph;
        }

        public override void Start()
        {
            Subgraph?.Start();
        }

        public override Status Update()
        {
            Subgraph?.Update();
            return Subgraph?.Status ?? Status.Error;
        }

        public override void Stop()
        {
            Subgraph?.Stop();
        }
    }
}
