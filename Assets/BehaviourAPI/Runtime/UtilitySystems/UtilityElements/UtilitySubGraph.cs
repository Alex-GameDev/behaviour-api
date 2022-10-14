using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    using Core;
    public class UtilitySubGraph : UtilityElement, ISubGraphContainer
    {
        private BehaviourEngine m_subGraph;
        private Factor m_mainFactor;
        public override int MaxOutputConnections => 1;
        public BehaviourEngine Subgraph { get => m_subGraph; set => m_subGraph = value; }

        public override void Update()
        {
            m_subGraph.Update();
        }

        protected override float ComputeUtility()
        {
            m_mainFactor.UpdateUtility();
            Utility = m_mainFactor.Utility;
            return Utility;
        }
    }
}