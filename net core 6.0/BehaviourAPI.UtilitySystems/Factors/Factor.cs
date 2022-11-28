namespace BehaviourAPI.UtilitySystems
{
    public abstract class Factor : UtilityNode
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type ChildType => typeof(Factor);

        #endregion
    }
}
