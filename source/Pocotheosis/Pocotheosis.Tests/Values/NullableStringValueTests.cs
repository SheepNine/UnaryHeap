using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    internal class NullableStringValueTests : PocoTestFixture<NullableStringValue>
    {
        public NullableStringValueTests()
        {
            AddSample(
                new NullableStringValue(string.Empty),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    MaybeString = ''
                }",
                @"{
                    ""MaybeString"": """"
                }");
            AddSample(
                new NullableStringValue("a regular string"),
                "652691c5e1d56b2b9e21d819d707025186f811d5a87442fb975b50babc3f1850",
                @"{
                    MaybeString = 'a regular string'
                }",
                @"{
                    ""MaybeString"": ""a regular string""
                }");
            AddSample(
                new NullableStringValue(null),
                "ad95131bc0b799c0b1af477fb14fcf26a6a9f76079e48bf090acb7e8367bfd0e",
                @"{
                    MaybeString = null
                }",
                @"{
                    ""MaybeString"": null
                }");
        }
    }
}
