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
                "8947bc952889a356fa03167e8a2e721866c8df002bab9b5d400991a38532d3c1",
                @"{
                    Strs = ()
                }",
                @"{
                    ""Strs"": {}
                }");
            AddSample(
                new StringMap(new Dataset() { { TrueBool.True, "bacon" } }),
                "8d4a0f0ff0973b00c240f38edc8ad8cfee6c230488b6a1b04a9fdeed699fdfbe",
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
                "b58d78377b088eab88cac3cffd796491da4f51f7500a2d9e660be1226f8f075c",
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

            AddInvalidConstructions(
                () => { var a = new StringMap(null); },
                () => { var a = new StringMap(new Dataset() { { TrueBool.True, null } }); }
            );
        }
    }
}
