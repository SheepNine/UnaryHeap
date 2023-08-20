using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    internal class PrimitiveValueTests : PocoTestFixture<PrimitiveValue>
    {
        public PrimitiveValueTests()
        {
            AddSample(
                new PrimitiveValue(0),
                "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
                @"{
                    Primitive = 0
                }",
                @"{
                    ""Primitive"": 0
                }");
            AddSample(
                new PrimitiveValue(128),
                "76be8b528d0075f7aae98d6fa57a6d3c83ae480a8469e668d7b0af968995ac71",
                @"{
                    Primitive = 128
                }",
                @"{
                    ""Primitive"": 128
                }");
            AddSample(
                new PrimitiveValue(255),
                "a8100ae6aa1940d0b663bb31cd466142ebbdbd5187131b92d93818987832eb89",
                @"{
                    Primitive = 255
                }",
                @"{
                    ""Primitive"": 255
                }");
        }
    }
}
