using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    internal class EmptyPocoTests : PocoTestFixture<EmptyPoco>
    {
        public EmptyPocoTests()
        {
            AddSample(
                new EmptyPoco(),
                "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
                @"{ }",
                @"{}");
        }
    }
}
