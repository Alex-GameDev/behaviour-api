using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    /// <summary>
    /// Interface implemented by the nodes and conections that executes a task.
    /// </summary>
    /// <typeparam name="T">The type of task (Action, Perception, etc)</typeparam>
    public interface ITaskHandler<T> where T : Task
    {
        public T Task { get; set; }
    }
}
