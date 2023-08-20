using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    internal class NullableClassValueTests : PocoTestFixture<NullableClassValue>
    {
        public NullableClassValueTests()
        {
            AddSample(
                new NullableClassValue(new PrimitiveValue(1)),
                "67c60c6612920fc8c68c55d63eadb34b0812235d7b2bf4f13f5692ed8f0cd856",
                @"{
                    MaybePoco = {
                        Primitive = 1
                    }
                }",
                @"{
                    ""MaybePoco"": {
                        ""Primitive"": 1
                    }
                }");
            AddSample(
                new NullableClassValue(new PrimitiveValue(20)),
                "8dadc1efc9bc46f8bb6f04179919b7ce9e307f28a36a2b0b2821f2bfd49c4671",
                @"{
                    MaybePoco = {
                        Primitive = 20
                    }
                }",
                @"{
                    ""MaybePoco"": {
                        ""Primitive"": 20
                    }
                }");
            AddSample(
                new NullableClassValue(null),
                "ad95131bc0b799c0b1af477fb14fcf26a6a9f76079e48bf090acb7e8367bfd0e",
                @"{
                    MaybePoco = null
                }",
                @"{
                    ""MaybePoco"": null
                }");
        }
    }
}
