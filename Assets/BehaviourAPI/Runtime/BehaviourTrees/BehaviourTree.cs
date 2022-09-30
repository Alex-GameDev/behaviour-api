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
        public override Type ConnectionType => typeof(BTConnection);

        BTNode m_rootNode;
        public BehaviourTree() { }

        public override void Start()
        {
            m_rootNode.Start();
        }

        public override void Update()
        {
            m_rootNode.Update();
            Status = m_rootNode.Status;
        }
    }
}
