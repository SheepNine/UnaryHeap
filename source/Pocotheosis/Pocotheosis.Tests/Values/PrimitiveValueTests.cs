using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    internal class PrimitiveValueTests : PocoTestFixture<PrimitiveValue>
    {
        public PrimitiveValueTests()
        {
            AddSample(
                new PrimitiveValue(0),
                "395c2f5598a1643a205154c6f4c46ce36895b28e6c35660a95e5c6fd5ef9aeab",
                @"{
                    Primitive = 0
                }",
                @"{
                    ""Primitive"": 0
                }");
            AddSample(
                new PrimitiveValue(128),
                "fa5c17001e2e4c1533a95db74671eeff8af4c6df273d2c88f53037493b64419b",
                @"{
                    Primitive = 128
                }",
                @"{
                    ""Primitive"": 128
                }");
            AddSample(
                new PrimitiveValue(255),
                "0176bf601df9b7d73d1dc25ce029d3db526358ffe604f26dd0adb9be8dd863b1",
                @"{
                    Primitive = 255
                }",
                @"{
                    ""Primitive"": 255
                }");
        }
    }
}
