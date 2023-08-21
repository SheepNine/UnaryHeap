using Pocotheosis.Tests.Pocos;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    internal class ClassArrayTests: PocoTestFixture<ClassArray>
    {
        public ClassArrayTests()
        {
            AddSample(
                new ClassArray(Enumerable.Empty<PrimitiveValue>()),
                "0bcb6d9b00c110d1020edc0fd875db044daba510d0acb48aeec7be8531172237",
                @"{
                    Pocos = []
                }",
                @"{
                    ""Pocos"": []
                }");
            AddSample(
                new ClassArray(new[] { new PrimitiveValue(88) }),
                "cc8e90ea2c23498855aedfa09f95ab199518f68c19dc3bc4f56073ddec431f0a",
                @"{
                    Pocos = [{
                        Primitive = 88
                    }]
                }",
                @"{
                    ""Pocos"": [{
                        ""Primitive"": 88
                    }]
                }");
            AddSample(
                new ClassArray(new[] { new PrimitiveValue(9), new PrimitiveValue(5) }),
                "1f7cc2d7b5a53b31f7eda6a07dc3492560c1e1ea805020f3a53dec25b2cea9eb",
                @"{
                    Pocos = [{
                        Primitive = 9
                    }, {
                        Primitive = 5
                    }]
                }",
                @"{
                    ""Pocos"": [{
                        ""Primitive"": 9
                    },{
                        ""Primitive"": 5
                    }]
                }");

            AddInvalidConstructions(
                () => { var a = new ClassArray(null); },
                () => { var a = new ClassArray(new PrimitiveValue[] { null }); }
            );
        }
    }
}
