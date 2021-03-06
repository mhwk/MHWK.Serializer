﻿using Microsoft.CodeAnalysis;
using Shouldly;

namespace MHWK.Serializer.Test
{
    public class SerializerGeneratorTest : GeneratorTestCase
    {
        public override ISourceGenerator Generator => new GenerateStructSerializers();

        public override string Source => @"
namespace MHWK.Serializer.Test
{
    [Serializable]
    public readonly struct Person
    {
        public readonly string Name;

        public readonly Gender Gender;

        public readonly Address Address;

        public Person(string name, Gender gender, Address address)
        {
            Name = name;
            Gender = gender;
            Address = address;
        }
    }

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

    [Serializable]
    public readonly struct Preferences
    {
        public readonly bool DarkMode;

        public readonly double Brightness;

        public readonly float Volume;

        public Preferences(bool darkMode, double brightness, float volume)
        {
            DarkMode = darkMode;
            Brightness = brightness;
            Volume = volume;
        }
    }
}";

        public override void Assert(GeneratorDriverRunResult result)
        {
            result.Diagnostics.ShouldBeEmpty();
        }
    }
}