using BehaviourAPI.Runtime.Core;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    public interface IUtilityHandler : IValueHandler<float>
    {
        public float Utility { get; }
        public void UpdateUtility();
    }
}