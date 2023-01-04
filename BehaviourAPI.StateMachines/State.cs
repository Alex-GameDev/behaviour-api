using BehaviourAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.StateMachines
{
    public abstract class State : FSMNode, IStatusHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override Type ChildType => typeof(Transition);

        public override int MaxInputConnections => -1;

        public Status Status
        {
            get => _status;
            protected set
            {
                if (_status != value)
                {
                    _status = value;
                    StatusChanged?.Invoke(_status);
                }
            }
        }

        public Action<Status> StatusChanged { get; set; }

        Status _status;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public abstract void Start();

        public abstract void Update();

        public abstract void Stop();

        #endregion
    }
}
