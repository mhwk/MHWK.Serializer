using Shouldly;
using Xunit;

namespace MHWK.Serializer.Example.Test
{
    public class PersonTest
    {
        [Fact]
        public void TestDeserialize()
        {
            var person = PersonSerializer.Instance.Deserialize(@"
{
    ""name"": ""Purno"",
    ""gender"": ""Male"",
    ""age"": 18,
    ""address"": {
        ""street"": ""FooBar Avenue"",
        ""houseNumber"": 666
    },
    ""preferences"": {
        ""darkMode"": true,
        ""brightness"": 0.4,
        ""volume"": 0.8
    }
}
");

            person.Name.ShouldBe("Purno");
            person.Gender.ShouldBe(Gender.Male);
            person.Address.Street.ShouldBe("FooBar Avenue");
            person.Address.HouseNumber.ShouldBe(666);
            person.Preferences.DarkMode.ShouldBeTrue();
            person.Preferences.Brightness.ShouldBe(.4);
            person.Preferences.Volume.ShouldBe(.8f);
        }
    }
}