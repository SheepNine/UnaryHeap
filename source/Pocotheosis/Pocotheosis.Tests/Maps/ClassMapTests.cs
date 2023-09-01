using Pocotheosis.Tests.Pocos;
using Dataset = System.Collections.Generic.Dictionary
        <string, Pocotheosis.Tests.Pocos.PrimitiveValue>;

namespace Pocotheosis.Tests.Maps
{
    internal class ClassMapTests : PocoTestFixture<ClassMap>
    {
        public ClassMapTests()
        {
            AddSample(
                new ClassMap(new Dataset()),
                "8b1fde09592443f1b32b4e08172bd374357872bdd6e048bc3ef540c32c6cc22c",
                @"{
                    Pocos = ()
                }",
                @"{
                    ""Pocos"": {}
                }");
            AddSample(
                new ClassMap(new Dataset() { { "bacon", P(25) } }),
                "9571ec336b0e28177be7517c7578d57e5ab705001e0f692058ac0abfa2b8de57",
                @"{
                    Pocos = (
                        'bacon' -> {
                            Primitive = 25
                        }
                    )
                }",
                @"{
                    ""Pocos"": {
                        ""bacon"": {
                            ""Primitive"": 25
                        }
                    }
                }");
            AddSample(
                new ClassMap(new Dataset() {
                    { "eggs", P(2) },
                    { "sausage", P(99) }
                }),
                "e7596ec3155e54bcb093055a5a76d4ea6a4ef624ea69438add44bcf4247117da",
                @"{
                    Pocos = (
                        'eggs' -> {
                            Primitive = 2
                        },
                        'sausage' -> {
                            Primitive = 99
                        }
                    )
                }",
                @"{
                    ""Pocos"": {
                        ""eggs"": {
                            ""Primitive"": 2
                        },
                        ""sausage"": {
                            ""Primitive"": 99
                        }
                    }
                }");

            AddInvalidConstructions(
                () => { var a = new ClassMap(null); },
                () => { var a = new ClassMap(new Dataset() { { "null", null } }); }
            );
        }
    }
}
