namespace BehaviourAPI.Core
{
    /// <summary>
    /// Basic element in a behaviour graph
    /// </summary>
    
    public abstract class GraphElement
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public virtual string Description => "No Description";

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        public behaviourGraph? BehaviourGraph;

        public string Name = string.Empty;

        #endregion

    }
}