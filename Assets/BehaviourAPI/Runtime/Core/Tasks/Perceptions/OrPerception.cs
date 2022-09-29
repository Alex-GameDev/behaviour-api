using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public class OrPerception : CompoundPerception
    {
        public override bool Check()
        {
            bool isAnyTrue = false;
            int idx = 0;
            while (!isAnyTrue && idx < Childs.Count)
            {
                isAnyTrue = Childs[idx].Check();
                idx++;
            }
            return isAnyTrue;
        }

        public override void Reset()
        {
            Childs.ForEach(child => child.Reset());
        }

        public override void Start()
        {
            Childs.ForEach(child => child.Start());
        }
    }
}