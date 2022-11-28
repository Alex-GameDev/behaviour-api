namespace BehaviourAPI.UtilitySystems
{
    using Core;
    public abstract class UtilityNode : Node, IUtilityHandler
    {

        #region ------------------------------------------ Properties -----------------------------------------

        public override int MaxInputConnections => -1;
        public float Utility { get => _utility; protected set => _utility = value; }
        float _utility;

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