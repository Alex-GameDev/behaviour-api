using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class CompoundPerception : Perception
    {
        public List<Perception> Childs { get; private set; }

    }
}