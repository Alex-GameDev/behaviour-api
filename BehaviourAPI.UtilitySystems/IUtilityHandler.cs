namespace BehaviourAPI.UtilitySystems
{
    public interface IUtilityHandler
    {
        public float Utility { get; }
        public void UpdateUtility();
    }
}