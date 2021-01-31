using System;
using System.Text.Json;

namespace StructSerializer.Test
{
    public class ManualPersonSerializer : Serializer<Person>
    {
        public static readonly ManualPersonSerializer Instance = new ManualPersonSerializer();

        private ManualPersonSerializer()
        {}
            
        public override Person Deserialize(ref Utf8JsonReader reader)
        {
            var name = default(string);
            var address = default(Address);
            
            reader.Read();
            if (reader.TokenType != JsonTokenType.StartObject) throw new ArgumentException("Expected start of object");
            
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    default: continue;
                    case JsonTokenType.PropertyName:
                        if (reader.GetString() == "name")
                        {
                            reader.Read();
                            if (reader.TokenType != JsonTokenType.String) throw new ArgumentException("Expected string for name");
                            name = reader.GetString();
                        }
                        if (reader.GetString() == "address")
                        {
                            reader.Read();
                            if (reader.TokenType != JsonTokenType.StartObject) throw new ArgumentException("Expected startObjct for bar");
                            name = reader.GetString();
                        }
                        break;

                }
            }

            return new Person(name, address);
        }
    }
}