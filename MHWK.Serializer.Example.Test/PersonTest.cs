using Xunit;
using Shouldly;

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
        ""darkMode"": true
    }
}
");

            person.Name.ShouldBe("Purno");
            person.Gender.ShouldBe(Gender.Male);
            person.Address.Street.ShouldBe("FooBar Avenue");
            person.Address.HouseNumber.ShouldBe(666);
            person.Preferences.DarkMode.ShouldBeTrue();
        }
    }
}