using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core.Actions
{
    public class FunctionalAction : Action
    {
        Func<Status> _update;

        System.Action? _start;
        System.Action? _stop;

        /// <summary>
        /// Create a <see cref="FunctionalAction"/> that executes a method on Start, a Function on Update and, optionally, a method on stop.
        /// </summary>
        /// <param name="update"></param>
        /// <param name="stop"></param>
        public FunctionalAction(System.Action start, Func<Status> update, System.Action? stop = null)
        {
            _start = start;
            _update = update;
            _stop = stop;
        }

        /// <summary>
        /// Create a <see cref="FunctionalAction"/> that executes a Function on Update and, optionally, a method on stop.
        /// </summary>
        /// <param name="update"></param>
        /// <param name="stop"></param>
        public FunctionalAction(Func<Status> update, System.Action? stop = null)
        {
            _update = update;
            _stop = stop;
        }

        /// <summary>
        /// Create a <see cref="FunctionalAction"/> that executes a method when started and only returns <see cref="Status.Success"/> on Update.
        /// (Commonly used on Transitions)
        /// </summary>
        /// <param name="start">The action</param>
        public FunctionalAction(System.Action start)
        {
            _start = start;
            _update = () => Status.Success;
        }

        public override void Start() => _start?.Invoke();
        public override Status Update() => _update.Invoke();
        public override void Stop() => _stop?.Invoke();
    }
}
