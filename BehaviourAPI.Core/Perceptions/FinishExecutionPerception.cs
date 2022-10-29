using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Perception used to check the IStatusHandler's finish execution status.
    /// </summary>
    public class FinishExecutionPerception : Perception
    {
        IStatusHandler _statusHandler;

        bool _triggerOnSuccess;
        bool _triggerOnFailure;

        public FinishExecutionPerception(IStatusHandler statusHandler, bool triggerOnSuccess, bool triggerOnFailure)
        {
            _statusHandler = statusHandler;
            _triggerOnSuccess = triggerOnSuccess;
            _triggerOnFailure = triggerOnFailure;
        }

        public override bool Check()
        {
            return ((_statusHandler.Status == Status.Sucess && _triggerOnSuccess) ||
                (_statusHandler.Status == Status.Failure && _triggerOnFailure));
        }
    }
}
