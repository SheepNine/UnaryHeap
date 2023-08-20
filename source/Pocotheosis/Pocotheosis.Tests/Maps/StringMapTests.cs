using Pocotheosis.Tests.Pocos;
using Dataset = System.Collections.Generic.Dictionary
    <Pocotheosis.Tests.Pocos.TrueBool, string>;

namespace Pocotheosis.Tests.Maps
{
    internal class StringMapTests : PocoTestFixture<StringMap>
    {
        public StringMapTests()
        {
            AddSample(
                new StringMap(new Dataset()),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    Strs = ()
                }",
                @"{
                    ""Strs"": {}
                }");
            AddSample(
                new StringMap(new Dataset() { { TrueBool.True, "bacon" } }),
                "535486a3fc2e2fc5b42d8adc6067c18313da9ab6f127f9fa8128d312ad8afaf7",
                @"{
                    Strs = (
                        True -> 'bacon'
                    )
                }",
                @"{
                    ""Strs"": {
                        ""True"": ""bacon""
                    }
                }");
            AddSample(
                new StringMap(new Dataset() {
                    { TrueBool.True, "eggs" },
                    { TrueBool.FileNotFound, "sausage" }
                }),
                "421e7081cb8c8ea6e588f593a8ed505e7900c1865c45772ffa2ec102666d8a2f",
                @"{
                    Strs = (
                        True -> 'eggs',
                        FileNotFound -> 'sausage'
                    )
                }",
                @"{
                    ""Strs"": {
                        ""True"": ""eggs"",
                        ""FileNotFound"": ""sausage""
                    }
                }");
        }
    }
}
