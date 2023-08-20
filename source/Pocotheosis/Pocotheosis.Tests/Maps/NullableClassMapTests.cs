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
                "7ee964d2c188056af5039adb6d8a3240a04b4c12b3b8b5f493c82de50d558839",
                @"{
                    MaybePocos = ()
                }",
                @"{
                    ""MaybePocos"": {}
                }");
            AddSample(
                new NullableClassMap(new Dataset() { { "bacon", new PrimitiveValue(25) } }),
                "d3360ab3dcc88d52d34add03b5300f56db12edcee9540f9b0af2af7d7640a834",
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
                "a6dda83a8ffe19ba3cc6a613d10a3fb72bb8d1aa499d96d5c405f6984e289116",
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
                "dd22d1301aa5be42dab8ae458632fd96735da538997ebeb71abc3538b821044e",
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
                "1522930c442b0c0db9a3173d85abb2aa6ceac9fd1688bdaae95f3dd8f801e548",
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
