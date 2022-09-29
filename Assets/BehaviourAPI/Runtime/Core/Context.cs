using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public class Context
    {
        public BehaviourRunner Runner { get; private set; }
        public Transform transform { get; private set; }
        public Rigidbody RigidBody { get; private set; }
        public BoxCollider BoxCollider { get; private set; }
    }
}