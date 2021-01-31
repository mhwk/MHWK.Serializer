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
    ""address"": {
        ""street"": ""FooBar Avenue"",
        ""houseNumber"": 666
    },
    ""age"": 18
}
");

            person.Name.ShouldBe("Purno");
            person.Gender.ShouldBe(Gender.Male);
            person.Address.Street.ShouldBe("FooBar Avenue");
            person.Address.HouseNumber.ShouldBe(666);
        }
    }
}