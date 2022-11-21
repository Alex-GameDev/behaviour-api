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
        public Status Status { get; protected set; }

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
        public abstract void Update();

        /// <summary>
        /// Finish the execution status giving 
        /// </summary>
        /// <param name="executionResult"></param>
        public void Finish(Status executionResult) => Status = executionResult;
    }
}
