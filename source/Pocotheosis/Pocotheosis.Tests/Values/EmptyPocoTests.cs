using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    internal class EmptyPocoTests : PocoTestFixture<EmptyPoco>
    {
        public EmptyPocoTests()
        {
            AddSample(
                new EmptyPoco(),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{ }",
                @"{}");
        }
    }
}
