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

        public FunctionalAction(System.Action? start, Func<Status> update, System.Action? stop = null)
        {
            _start = start;
            _update = update;
            _stop = stop;
        }

        public FunctionalAction(Func<Status> update, System.Action? stop = null)
        {
            _update = update;
            _stop = stop;
        }

        public override void Start() => _start?.Invoke();
        public override Status Update() => _update.Invoke();
        public override void Stop() => _stop?.Invoke();
    }
}
