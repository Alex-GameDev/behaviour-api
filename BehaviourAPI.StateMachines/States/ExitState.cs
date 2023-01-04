using BehaviourAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.StateMachines
{
    public class ExitState : State
    {
        public override int MaxInputConnections => -1;

        public override int MaxOutputConnections => 0;

        public Status ExitStatus;

        public override void Start()
        {
            Status = ExitStatus;
            BehaviourGraph.Finish(ExitStatus);
        }

        public override void Stop()
        {
            Status = Status.None;
        }

        public override void Update()
        {
            return;
        }
    }
}
