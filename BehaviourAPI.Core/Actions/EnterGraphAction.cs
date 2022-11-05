namespace BehaviourAPI.Core.Actions
{
    public class EnterGraphAction : Action
    {
        public BehaviourGraph? Subgraph;

        public EnterGraphAction(BehaviourGraph? subgraph)
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
