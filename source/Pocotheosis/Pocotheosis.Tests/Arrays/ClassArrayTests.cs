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
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    Pocos = []
                }",
                @"{
                    ""Pocos"": []
                }");
            AddSample(
                new ClassArray(new[] { new PrimitiveValue(88) }),
                "1b5f88550bfee76a655c053a51ca66c550d9843a0103cc8da532ab65dd390c41",
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
                "bec1689854be1d7f5efbb0206cf65ffbc6abba6afacaf17c01b45caa871b3e56",
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
        }
    }
}
