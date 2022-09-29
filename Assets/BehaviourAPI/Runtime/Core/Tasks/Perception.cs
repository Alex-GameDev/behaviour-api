using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class Perception : Task
    {
        public abstract bool Check();
    }
}