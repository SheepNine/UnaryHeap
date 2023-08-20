using Pocotheosis.Tests.Pocos;
using Dataset = System.Collections.Generic.Dictionary<byte, Pocotheosis.Tests.Pocos.TrueBool>;

namespace Pocotheosis.Tests.Maps
{
    internal class EnumMapTests : PocoTestFixture<EnumMap>
    {
        public EnumMapTests()
        {
            AddSample(
                new EnumMap(new Dataset()),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    Enums = ()
                }",
                @"{
                    ""Enums"": []
                }");
            AddSample(
                new EnumMap(new Dataset() { { 5, TrueBool.True} }),
                "ea6e7853f72eb44390df9514313d5753a16d24b4d1373869cb216fdf842f4a9a",
                @"{
                    Enums = (
                        5 -> True
                    )
                }",
                @"{
                    ""Enums"": [{
                        ""k"": 5,
                        ""v"": ""True""
                    }]
                }");
            AddSample(
                new EnumMap(new Dataset() {
                    { 5, TrueBool.True },
                    { 77, TrueBool.FileNotFound }
                }),
                "7868b2d0c5f04606cca6910ed9d09553860bb63d529a67baa497fb0d65151830",
                @"{
                    Enums = (
                        5 -> True,
                        77 -> FileNotFound
                    )
                }",
                @"{
                    ""Enums"": [{
                        ""k"": 5,
                        ""v"": ""True""
                    },{
                        ""k"": 77,
                        ""v"": ""FileNotFound""
                    }]
                }");
        }
    }
}
