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
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    Pocos = ()
                }",
                @"{
                    ""Pocos"": {}
                }");
            AddSample(
                new ClassMap(new Dataset() { { "bacon", new PrimitiveValue(25) } }),
                "8ce10f81e558b82801cdf8d0b51985980438a01ce9e7e579ecf5005bf2f4af91",
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
                    { "eggs", new PrimitiveValue(2) },
                    { "sausage", new PrimitiveValue(99) }
                }),
                "8cdc881b1b0f8b72918f4a65b9ecdf8a444a6382660287274d79a5210632f6d9",
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
        }
    }
}
