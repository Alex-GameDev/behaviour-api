namespace BehaviourAPI.UtilitySystems
{
    public interface IUtilityHandler
    {
        float Utility { get; }
        void UpdateUtility();
    }
}