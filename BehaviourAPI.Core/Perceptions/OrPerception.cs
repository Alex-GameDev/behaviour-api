namespace BehaviourAPI.Core.Perceptions
{
    public class OrPerception : CompoundPerception
    {
        public OrPerception(List<Perception> perceptions) : base(perceptions) { }

        public OrPerception(params Perception[] perceptions) : base(perceptions) { }

        public override bool Check()
        {
            if (Perceptions.Count == 0) return false;

            bool result = false;
            int idx = 0;
            while (result == false && idx < Perceptions.Count)
            {
                result = Perceptions[idx].Check();
                idx++;
            }
            return result;
        }
    }
}
