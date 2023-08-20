using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    internal class EnumValueTests : PocoTestFixture<EnumValue>
    {
        public EnumValueTests()
        {
            AddSample(
                new EnumValue(TrueBool.True),
                "6e340b9cffb37a989ca544e6bb780a2c78901d3fb33738768511a30617afa01d",
                @"{
                    Enum = True
                }",
                @"{
                    ""Enum"": ""True""
                }");
            AddSample(
                new EnumValue(TrueBool.False),
                "4bf5122f344554c53bde2ebb8cd2b7e3d1600ad631c385a5d7cce23c7785459a",
                @"{
                    Enum = False
                }",
                @"{
                    ""Enum"": ""False""
                }");
            AddSample(
                new EnumValue(TrueBool.FileNotFound),
                "beead77994cf573341ec17b58bbf7eb34d2711c993c1d976b128b3188dc1829a",
                @"{
                    Enum = FileNotFound
                }",
                @"{
                    ""Enum"": ""FileNotFound""
                }");
        }
    }
}
