using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime
{
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class TaskMethodAttribute : System.Attribute
    {
        public TaskMethodAttribute()
        {
        }
    }
}
