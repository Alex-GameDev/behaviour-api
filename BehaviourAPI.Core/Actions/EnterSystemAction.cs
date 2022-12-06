using BehaviourAPI.Core.Exceptions;

namespace BehaviourAPI.Core.Actions
{
    public class EnterSystemAction : Action
    {
        public BehaviourSystem SubSystem;

        /// <summary>
        /// True if the subsystem will restart after finish
        /// </summary>
        public bool ExecuteOnLoop;

        /// <summary>
        /// True if the subsystem won't restart if is interrupted
        /// </summary>
        public bool DontStopOnInterrupt;

        public EnterSystemAction(BehaviourSystem subSystem, bool executeOnLoop = false, bool dontStopOnInterrupt = false)
        {
            SubSystem = subSystem;
            ExecuteOnLoop = executeOnLoop;
            DontStopOnInterrupt = dontStopOnInterrupt;
        }

        public override void Start()
        {
            if (SubSystem == null)
                throw new MissingSubsystemException(this, "Subsystem cannot be null");

            if (DontStopOnInterrupt && SubSystem.Status == Status.None) return;

            SubSystem?.Start();
        }

        public override Status Update()
        {
            if (SubSystem == null)
                throw new MissingSubsystemException(this, "Subsystem cannot be null");

            if (ExecuteOnLoop && SubSystem.Status != Status.Running)
                SubSystem.Restart();

            SubSystem.Update();
            return SubSystem.Status;
        }

        public override void Stop()
        {
            if (SubSystem == null)
                throw new MissingSubsystemException(this, "Subsystem cannot be null");

            if (DontStopOnInterrupt && SubSystem.Status == Status.Running) return;
            
            SubSystem?.Stop();
        }
    }
}
