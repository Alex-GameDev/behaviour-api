namespace BehaviourAPI.Core
{
    /// <summary>
    /// Basic element in a behaviour graph
    /// </summary>
    
    public abstract class GraphElement
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public BehaviourGraph? BehaviourGraph { get; set; }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        public string Name = string.Empty;
        
        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Build the internal references
        /// </summary>
        public virtual void Initialize()
        {
        }

        #endregion

    }
}