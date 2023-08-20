using Pocotheosis.Tests.Pocos;
using Dataset = System.Collections.Generic.Dictionary
    <string, Pocotheosis.Tests.Pocos.PrimitiveValue>;

namespace Pocotheosis.Tests.Maps
{
    internal class NullableClassMapTests : PocoTestFixture<NullableClassMap>
    {
        public NullableClassMapTests()
        {
            AddSample(
                new NullableClassMap(new Dataset()),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    MaybePocos = ()
                }",
                @"{
                    ""MaybePocos"": {}
                }");
            AddSample(
                new NullableClassMap(new Dataset() { { "bacon", new PrimitiveValue(25) } }),
                "25b625d55f7547da690c82544642c01d7352593548e15fff92e7976e4cad52d0",
                @"{
                    MaybePocos = (
                        'bacon' -> {
                            Primitive = 25
                        }
                    )
                }",
                @"{
                    ""MaybePocos"": {
                        ""bacon"": {
                            ""Primitive"": 25
                        }
                    }
                }");
            AddSample(
                new NullableClassMap(new Dataset() { { "urbacon", null } }),
                "f3f7cb254b14d220b51b4449aaaa6c0f89178d422e8d2847bad962cb28984374",
                @"{
                    MaybePocos = (
                        'urbacon' -> null
                    )
                }",
                @"{
                    ""MaybePocos"": {
                        ""urbacon"": null
                    }
                }");
            AddSample(
                new NullableClassMap(new Dataset() {
                    { "eggs", new PrimitiveValue(2) },
                    { "sausage", new PrimitiveValue(99) }
                }),
                "bc54eaf99fecaaf9272b0940023f570ff593650afce8f27ac57eabb57683bded",
                @"{
                    MaybePocos = (
                        'eggs' -> {
                            Primitive = 2
                        },
                        'sausage' -> {
                            Primitive = 99
                        }
                    )
                }",
                @"{
                    ""MaybePocos"": {
                        ""eggs"": {
                            ""Primitive"": 2
                        },
                        ""sausage"": {
                            ""Primitive"": 99
                        }
                    }
                }");
            AddSample(
                new NullableClassMap(new Dataset() {
                    { "eggs", null },
                    { "sausage", new PrimitiveValue(101) }
                }),
                "e3dc6ee76d59f4c29d036d5fd9d098fe13ec059b31154351d6f0a487380ecd83",
                @"{
                    MaybePocos = (
                        'eggs' -> null,
                        'sausage' -> {
                            Primitive = 101
                        }
                    )
                }",
                @"{
                    ""MaybePocos"": {
                        ""eggs"": null,
                        ""sausage"": {
                            ""Primitive"": 101
                        }
                    }
                }");
        }
    }
}
