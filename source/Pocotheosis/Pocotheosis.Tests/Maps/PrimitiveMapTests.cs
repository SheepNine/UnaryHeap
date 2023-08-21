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
                "5fbbc50a5e0bc4e1bb5ea4bbacda5ae6709eff8e286fb002fee73a4913820fe9",
                @"{
                    Primitives = ()
                }",
                @"{
                    ""Primitives"": []
                }");
            AddSample(
                new PrimitiveMap(new Dataset() { { true, 5 } }),
                "47c678986f11bdfdd3d0e0c74e4ecc76ca14e899350c7b42f82a1e8ef2cb0899",
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
                "b43d811d1b015703114f66f8c523920b475caea5cd035c685769bea0c76c7450",
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

            AddInvalidConstructions(
                () => { var a = new PrimitiveMap(null); }
            );
        }
    }
}
