using System;
using System.Text.Json;

namespace MHWK.Serializer.Example.Test
{
    public sealed class ManualGenderSerializer : Serializer<Gender>
    {
        public override Gender Deserialize(ref Utf8JsonReader reader)
        {
            reader.Read();
            
            if (reader.TokenType != JsonTokenType.String) throw new ArgumentException($"Expected string for enum {typeof(Gender).FullName}");

            return (Gender) Enum.Parse(typeof(Gender), reader.GetString(), true);
        }
    }
}