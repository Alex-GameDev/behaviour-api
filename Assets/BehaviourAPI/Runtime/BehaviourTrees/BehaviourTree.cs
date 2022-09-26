using System;
using System.Collections.Generic;
using BehaviourAPI.Runtime.Core;
using UnityEngine;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using Core;
    /// <summary>
    /// Decision system that consists of traversing a tree in depth depending on the result returned by its nodes.
    /// </summary>
    public class BehaviourTree : BehaviourEngine
    {
        public override Type NodeType => typeof(BTNode);
        public Status RootStatus => StartNode.Status;
        public BehaviourTree() { }
    }
}
