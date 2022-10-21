using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    /// <summary>
    /// Task that can be checked and return a boolean
    /// </summary>
    public abstract class Perception : Task
    {
        /// <summary>
        /// Check if the perception is being triggered.
        /// </summary>
        /// <returns>True is the perception is triggered, false otherwise</returns>
        public abstract bool Check();
    }
}