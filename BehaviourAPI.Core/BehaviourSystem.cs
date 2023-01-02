using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core
{
    public abstract class BehaviourSystem : IStatusHandler
    {
        /// <summary>
        /// The current execution status of the graph.
        /// </summary>
        public Status Status 
        {
            get => _status; 
            protected set
            {
                if(_status != value)
                {
                    _status = value;
                    StatusChanged?.Invoke(_status);
                }
            }
        }

        public Action<Status> StatusChanged { get; set; }


        Status _status;

        /// <summary>
        /// Enter this behavior graph
        /// </summary>
        public virtual void Start()
        {
            if (Status != Status.None)
                throw new Exception("ERROR: This graph is already been executed");

            Status = Status.Running;
        }

        /// <summary>
        /// Executes every frame
        /// </summary>
        public virtual void Stop()
        {
            if (Status == Status.None)
                throw new Exception("ERROR: This graph is already been stopped");

            Status = Status.None;
        }

        /// <summary>
        /// Executes the last execution frame
        /// </summary>
        public void Update()
        {
            if (Status != Status.Running) return; // Graph already finished
            Execute();                
        }

        /// <summary>
        /// Finish the execution status giving 
        /// </summary>
        /// <param name="executionResult"></param>
        public void Finish(Status executionResult) => Status = executionResult;

        public abstract void Execute();

        public void Restart()
        {
            Stop();
            Start();
        }
    }
}
