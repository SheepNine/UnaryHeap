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
                "9d440eeeca9570b4064dca02133b3c4960bdc7615e04d024d61b3a897c37d5df",
                @"{
                    Enums = ()
                }",
                @"{
                    ""Enums"": []
                }");
            AddSample(
                new EnumMap(new Dataset() { { 5, TrueBool.True} }),
                "83df55090bcaa83e2ee0e8dba3e20d341edc9448ddfa7a4953b0a1cbfd66a5ad",
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
                "33608851f15836fd3161258093aa66b8f2e827504f3a92556ab6ebd002cffa33",
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
