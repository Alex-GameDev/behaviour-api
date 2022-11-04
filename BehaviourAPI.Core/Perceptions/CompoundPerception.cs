using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Perception that is compound itself by multiple perceptions.
    /// </summary>
    public abstract class CompoundPerception : Perception
    {
        public List<Perception> Perceptions;

        public CompoundPerception(List<Perception> perceptions)
        {
            Perceptions = perceptions;
        }

        public CompoundPerception(params Perception[] perceptions)
        {
            Perceptions = perceptions.ToList();
        }

        public override void Start()
        {
            Perceptions.ForEach(p => p.Start());
        }

        public override void Stop()
        {
            Perceptions.ForEach(p => p.Stop());
        }
    }
}
