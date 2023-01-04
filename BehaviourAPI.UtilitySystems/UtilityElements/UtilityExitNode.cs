using BehaviourAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.UtilitySystems.UtilityElements
{
    public class UtilityExitNode : UtilityExecutableNode
    {
        #region ------------------------------------------ Properties ----------------------------------------

        public Status ExitStatus;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override bool FinishExecutionWhenActionFinishes()
        {
            return true;
        }

        public override void Start()
        {
            Status = ExitStatus;
            BehaviourGraph.Finish(ExitStatus);
        }

        public override void Stop()
        {
            Status = Status.None;
        }

        public override void Update()
        {
            return;
        }

        #endregion
    }
}
