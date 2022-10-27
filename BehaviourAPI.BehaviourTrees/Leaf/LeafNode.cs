namespace BehaviourAPI.BehaviourTrees
{
    /// <summary>
    /// BTNode type that has no children.
    /// </summary>
    public abstract class LeafNode : BTNode
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public sealed override int MaxOutputConnections => 0;

        #endregion

    }
}
