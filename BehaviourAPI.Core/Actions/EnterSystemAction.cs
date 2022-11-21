namespace BehaviourAPI.Core.Actions
{
    public class EnterSystemAction : Action
    {
        public BehaviourSystem? Subgraph;

        /// <summary>
        /// True if the subsystem will restart after finish
        /// </summary>
        public bool ExecuteOnLoop;

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
            if(ExecuteOnLoop && Subgraph?.Status != Status.Running) Subgraph?.Restart();

            Subgraph?.Update();
            return Subgraph?.Status ?? Status.Error;
        }

        public override void Stop()
        {
            Subgraph?.Stop();
        }
    }
}
