using System;
using System.Text;
using System.Text.Json;

namespace MHWK.Serializer
{
    public abstract class Serializer<T>
    {
        public T Deserialize(string json)
        {
            return Deserialize(Encoding.UTF8.GetBytes(json));
        }
        
        public T Deserialize(ReadOnlySpan<byte> json)
        {
            var reader = new Utf8JsonReader(json);
            return Deserialize(ref reader);
        }
        
        public abstract T Deserialize(ref Utf8JsonReader reader);
    }
}