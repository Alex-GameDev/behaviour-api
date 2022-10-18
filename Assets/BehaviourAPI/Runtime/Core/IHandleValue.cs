using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviourAPI.Runtime.Core
{
    public interface IValueHandler<T>
    {
        public Action<T> OnValueChanged { get; set; }
    }
}
