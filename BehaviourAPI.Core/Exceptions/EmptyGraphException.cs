using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core.Exceptions
{
    public class EmptyGraphException : Exception
    {
        public BehaviourGraph Graph { get; private set; }
        public EmptyGraphException(BehaviourGraph graph)
        {
            Graph = graph;
        }

        public EmptyGraphException(BehaviourGraph graph, string message) : base(message)
        {
            Graph = graph;
        }

        public EmptyGraphException(BehaviourGraph graph, string message, Exception innerException) : base(message, innerException)
        {
            Graph = graph;
        }

        protected EmptyGraphException(BehaviourGraph graph, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Graph = graph;
        }
    }
}
