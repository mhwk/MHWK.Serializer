using System;
using System.Text;
using System.Text.Json;

namespace StructSerializer
{
    public abstract class Serializer<T>
    {
        public T Deserialize(string json)
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            return Deserialize(reader);
        }
        
        public abstract T Deserialize(Utf8JsonReader reader);
    }
}