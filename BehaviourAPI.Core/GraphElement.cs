using System.Text.Json;

namespace BehaviourAPI.Core
{
    /// <summary>
    /// Basic element in a behaviour graph
    /// </summary>
    
    public abstract class GraphElement : ISerializableElement
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

        public virtual void SerializeToJSON(Utf8JsonWriter writer)
        {
            writer.WriteString("_type", GetType().FullName);            
        }

        public virtual void DeserializeFromJSON(ref Utf8JsonReader reader)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}