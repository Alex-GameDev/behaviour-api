using System;

namespace BehaviourAPI.Core.Actions
{
    public abstract class Action
    {
        public abstract void Start();
        public abstract Status Update();
        public abstract void Stop();

        public static implicit operator Action(Func<Status> func) => new FunctionalAction(func);
        public static implicit operator Action(System.Action action) => new FunctionalAction(action);
        public static implicit operator Action(Status status) => new FunctionalAction(() => status);
        public static implicit operator Action(BehaviourSystem behaviourSystem) => new EnterSystemAction(behaviourSystem);
    }
}
