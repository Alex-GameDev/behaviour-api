using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core.Exceptions
{
    public class MissingChildException : NullReferenceException
    {
        public Node Node;

        public MissingChildException(Node node)
        {
            Node = node;
        }

        public MissingChildException(Node node,string message) : base(message)
        {
            Node = node;
        }

        public MissingChildException(Node node, string message, Exception innerException) : base(message, innerException)
        {
            Node = node;
        }

        protected MissingChildException(Node node, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Node = node;
        }
    }
}
