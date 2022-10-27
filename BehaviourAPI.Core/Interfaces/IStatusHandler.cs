namespace BehaviourAPI.Core
{
    public interface IStatusHandler
    {
        public Status Status { get; }

        /// <summary>
        /// Called the first frame of the execution
        /// </summary>
        public void Start();

        /// <summary>
        /// Called every execution frame
        /// </summary>
        public void Update();

        /// <summary>
        /// Called when execution ends.
        /// </summary>
        public void Stop();
    }

}