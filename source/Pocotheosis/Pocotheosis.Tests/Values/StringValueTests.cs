using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    internal class StringValueTests : PocoTestFixture<StringValue>
    {
        public StringValueTests()
        {
            AddSample(
                new StringValue(string.Empty),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    Str = ''
                }",
                @"{
                    ""Str"": """"
                }");
            AddSample(
                new StringValue("a regular string"),
                "652691c5e1d56b2b9e21d819d707025186f811d5a87442fb975b50babc3f1850",
                @"{
                    Str = 'a regular string'
                }",
                @"{
                    ""Str"": ""a regular string""
                }");
        }
    }
}
