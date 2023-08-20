using Pocotheosis.Tests.Pocos;
using Dataset = System.Collections.Generic.Dictionary<bool, byte>;

namespace Pocotheosis.Tests.Maps
{
    internal class PrimitiveMapTests: PocoTestFixture<PrimitiveMap>
    {
        public PrimitiveMapTests()
        {
            AddSample(
                new PrimitiveMap(new Dataset()),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    Primitives = ()
                }",
                @"{
                    ""Primitives"": []
                }");
            AddSample(
                new PrimitiveMap(new Dataset() { { true, 5 } }),
                "68d8cba3e752f084290957002386d0ac3097d9c3625c2dc4ece4bac24f6e68df",
                @"{
                    Primitives = (
                        True -> 5
                    )
                }",
                @"{
                    ""Primitives"": [{
                        ""k"": true,
                        ""v"": 5
                    }]
                }");
            AddSample(
                new PrimitiveMap(new Dataset() { { true, 5 }, { false, 77 } }),
                "1e0b135ea06c57c9573af63b839ab30bc2d5de928e38856d4f2a42c93cbe7e01",
                @"{
                    Primitives = (
                        False -> 77,
                        True -> 5
                    )
                }",
                @"{
                    ""Primitives"": [{
                        ""k"": false,
                        ""v"": 77
                    },{
                        ""k"": true,
                        ""v"": 5
                    }]
                }");
        }
    }
}
