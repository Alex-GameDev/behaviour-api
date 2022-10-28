using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core.Actions
{
    public abstract class Action
    {
        public abstract void Start();
        public abstract Status Update();
        public abstract void Stop();
    }
}
