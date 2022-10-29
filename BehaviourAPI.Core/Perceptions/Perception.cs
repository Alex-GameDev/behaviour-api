using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core.Perceptions
{
    public abstract class Perception
    {
        public virtual void Start() { }
        public abstract bool Check();
        public virtual void Stop() { }
    }
}
