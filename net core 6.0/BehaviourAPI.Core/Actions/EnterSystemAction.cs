namespace BehaviourAPI.Core.Actions
{
    public class EnterSystemAction : Action
    {
        public BehaviourSystem? SubSystem;

        /// <summary>
        /// True if the subsystem will restart after finish
        /// </summary>
        public bool ExecuteOnLoop;

        /// <summary>
        /// True if the subsystem won't restart if is interrupted
        /// </summary>
        public bool DontStopOnInterrupt;

        public EnterSystemAction(BehaviourSystem? subSystem, bool executeOnLoop = false, bool dontStopOnInterrupt = false)
        {
            SubSystem = subSystem;
            ExecuteOnLoop = executeOnLoop;
            DontStopOnInterrupt = dontStopOnInterrupt;
        }

        public override void Start()
        {
            if (DontStopOnInterrupt && SubSystem?.Status == Status.None) return;

            SubSystem?.Start();
        }

        public override Status Update()
        {
            if(ExecuteOnLoop && SubSystem?.Status != Status.Running) SubSystem?.Restart();

            SubSystem?.Update();
            return SubSystem?.Status ?? Status.Error;
        }

        public override void Stop()
        {
            if (DontStopOnInterrupt && SubSystem?.Status == Status.Running) return;
            
            SubSystem?.Stop();
        }
    }
}
