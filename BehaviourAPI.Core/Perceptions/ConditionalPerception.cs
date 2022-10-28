using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core.Perceptions
{
    public class ConditionalPerception : Perception
    {
        Func<bool> _check;
        System.Action? _start;
        System.Action? _stop;

        public ConditionalPerception(System.Action? start, Func<bool> update, System.Action? stop = null)
        {
            _start = start;
            _check = update;
            _stop = stop;
        }

        public ConditionalPerception(Func<bool> check, System.Action? stop = null)
        {
            _check = check;
            _stop = stop;
        }

        public override void Start() => _start?.Invoke();
        public override bool Check() => _check.Invoke();
        public override void Stop() => _stop?.Invoke();
    }
}
