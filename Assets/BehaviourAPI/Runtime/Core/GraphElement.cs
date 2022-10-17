using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class GraphElement : ScriptableObject
    {
        [HideInInspector] public BehaviourEngine BehaviourGraph;
        public virtual string Description => "No description";
        public virtual string Name => "Node";
    }
}
