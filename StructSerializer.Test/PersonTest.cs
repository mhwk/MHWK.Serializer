using Shouldly;
using Xunit;

namespace StructSerializer.Test
{
    public class PersonTest
    {
        [Fact]
        public void TestDeserialize()
        {
            var person = PersonSerializer.Instance.Deserialize(@"
{
    ""name"": ""Purno"",
    ""address"": {
        ""street"": ""FooBar Avenue"",
        ""houseNumber"": 666
    },
    ""age"": 18
}
");

            person.Name.ShouldBe("Purno");
            person.Address.Street.ShouldBe("FooBar Avenue");
            person.Address.HouseNumber.ShouldBe(666);
        }
    }
}