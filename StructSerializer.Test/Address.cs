using System;

namespace StructSerializer.Test
{
    [Serializable]
    public readonly struct Address
    {
        public readonly string Street;

        public readonly int HouseNumber;

        public Address(string street, int houseNumber)
        {
            Street = street;
            HouseNumber = houseNumber;
        }
    }
}