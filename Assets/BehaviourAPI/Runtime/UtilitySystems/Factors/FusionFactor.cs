using System;
using System.Collections.Generic;
using BehaviourAPI.Runtime.Core;
using UnityEngine;

namespace BehaviourAPI.Runtime.UtilitySystems
{
    using Core;

    public abstract class FusionFactor : Factor
    {
        public override int MaxOutputConnections => -1;

        protected List<Factor> m_childFactors;
    }
}
