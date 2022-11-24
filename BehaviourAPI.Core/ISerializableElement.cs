using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BehaviourAPI.Core
{
    internal interface ISerializableElement
    {
        public void SerializeToJSON(Utf8JsonWriter writer);
        public void DeserializeFromJSON(ref Utf8JsonReader reader);
    }
}
