using System;

namespace StructSerializer.Test
{
    [Serializable]
    public readonly struct Person
    {
        public readonly string Name;

        public readonly Address Address;

        public Person(string name, Address address)
        {
            Name = name;
            Address = address;
        }
    }
}