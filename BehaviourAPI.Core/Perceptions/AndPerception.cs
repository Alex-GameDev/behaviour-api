namespace BehaviourAPI.Core.Perceptions
{
    public class AndPerception : CompoundPerception
    {
        public override bool Check()
        {
            if(Perceptions.Count == 0) return false;

            bool result = true;
            int idx = 0;
            while(result == true && idx < Perceptions.Count)
            {
                result = Perceptions[idx].Check();
            }
            return result;
        }
    }
}
