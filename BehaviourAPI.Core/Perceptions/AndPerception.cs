using System.Collections.Generic;

namespace BehaviourAPI.Core.Perceptions
{
    public class AndPerception : CompoundPerception
    {
        public AndPerception(List<Perception> perceptions) : base(perceptions) { }

        public AndPerception(params Perception[] perceptions) : base(perceptions) { }

        public override bool Check()
        {
            if(Perceptions.Count == 0) return false;

            bool result = true;
            int idx = 0;
            while(result == true && idx < Perceptions.Count)
            {
                result = Perceptions[idx].Check();
                idx++;
            }
            return result;
        }
    }
}
