namespace BehaviourAPI.Runtime.Core
{
    public interface IStatusHandler : IValueHandler<Status>
    {
        public Status Status { get; }
    }

}