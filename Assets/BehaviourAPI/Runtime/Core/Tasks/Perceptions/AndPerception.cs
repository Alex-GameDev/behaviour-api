using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public class AndPerception : CompoundPerception
    {
        public override bool Check()
        {
            bool isAllTrue = true;
            int idx = 0;
            while (isAllTrue && idx < Childs.Count)
            {
                isAllTrue = Childs[idx].Check();
                idx++;
            }
            return isAllTrue;
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