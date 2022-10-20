using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public interface IActionAsignable
    {
        public ActionTask Action { get; set; }
    }
}
