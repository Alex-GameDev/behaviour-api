using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public interface IPerceptionAssignable
    {
        public Perception Perception { get; set; }
    }
}
