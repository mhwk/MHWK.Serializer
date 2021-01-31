using System;

namespace MHWK.Serializer.Example
{
    [Serializable]
    public readonly struct Person
    {
        public readonly string Name;

        public readonly Gender Gender;

        public readonly Address Address;
        
        public readonly Preferences Preferences;

        public Person(string name, Gender gender, Address address, Preferences preferences)
        {
            Name = name;
            Gender = gender;
            Address = address;
            Preferences = preferences;
        }
    }
}