namespace BehaviourAPI.UtilitySystems
{
    using BehaviourAPI.Core.Actions;
    using Core;
    using System;

    public abstract class UtilityNode : Node, IUtilityHandler
    {

        #region ------------------------------------------ Properties -----------------------------------------

        public override int MaxInputConnections => -1;
        public float Utility 
        { 
            get => _utility; 
            protected set
            {
                if(_utility != value)
                {
                    _utility = value;
                    UtilityChanged?.Invoke(_utility);
                }
            }
        }

        float _utility;

        public Action<float> UtilityChanged { get; set; }
        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// Updates the current value of <see cref="Utility"/>
        /// </summary>
        public void UpdateUtility()
        {
            Utility = ComputeUtility();
        }

        protected abstract float ComputeUtility();

        #endregion
    }
}