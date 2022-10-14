using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    using Core;

    /// <summary>
    /// Behaviour graph that choose between diferent <see cref="UtilityElement"/> items and executes.
    /// </summary>
    public class UtilitySystem : BehaviourEngine
    {
        public float Inertia = 1.3f;
        public override Type NodeType => typeof(UtilityNode);
        public override Type ConnectionType => typeof(UtilityConnection);

        List<UtilityElement> m_utilityElements;
        UtilityElement m_currentBestAction;

        public override void Start()
        {

        }

        public override void Update()
        {
            m_currentBestAction = ComputeCurrentBestAction();
            m_currentBestAction.Update();
        }

        private UtilityElement ComputeCurrentBestAction()
        {
            bool lockCurrentSelectedElement = false;
            float currentHigherUtility = 0f;
            if (m_currentBestAction != null)
            {
                m_currentBestAction.UpdateUtility();
                currentHigherUtility = m_currentBestAction.Utility * Inertia;
                if (currentHigherUtility >= 1f) lockCurrentSelectedElement = true;
            }

            var newBestAction = m_currentBestAction;
            for (int i = 0; i < m_utilityElements.Count; i++)
            {
                if (m_utilityElements[i] != m_currentBestAction)
                {
                    m_utilityElements[i].UpdateUtility();
                    var utility = m_utilityElements[i].Utility;

                    if (!lockCurrentSelectedElement && utility > currentHigherUtility)
                    {
                        newBestAction = m_utilityElements[i];
                        if (utility >= 1f) lockCurrentSelectedElement = true;
                    }
                }
            }
            return newBestAction;
        }
    }
}