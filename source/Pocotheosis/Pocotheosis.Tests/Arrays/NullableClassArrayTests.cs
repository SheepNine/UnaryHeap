using Pocotheosis.Tests.Pocos;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    internal class NullableClassArrayTests : PocoTestFixture<NullableClassArray>
    {
        public NullableClassArrayTests()
        {
            AddSample(
                new NullableClassArray(Enumerable.Empty<PrimitiveValue>()),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    MaybePocos = []
                }",
                @"{
                    ""MaybePocos"": []
                }");
            AddSample(
                new NullableClassArray(new[] { new PrimitiveValue(88) }),
                "a8223ec3b9203ce43d5d0762bcfa84c954f72f60dea9db9f9599aa1198db5ae4",
                @"{
                    MaybePocos = [{
                        Primitive = 88
                    }]
                }",
                @"{
                    ""MaybePocos"": [{
                        ""Primitive"": 88
                    }]
                }");
            AddSample(
                new NullableClassArray(new PrimitiveValue[] { null }),
                "b15348c8f462384c01e83b6d499c6faf3f96808f5aa07c6bab4b65b36b4445d4",
                @"{
                    MaybePocos = [null]
                }",
                @"{
                    ""MaybePocos"": [null]
                }");
            AddSample(
                new NullableClassArray(new[] { new PrimitiveValue(9), new PrimitiveValue(5) }),
                "28a195f8cdf2f5caf46006a8fdd9059dd1b02cf611bca7af35bff9ca40a71243",
                @"{
                    MaybePocos = [{
                        Primitive = 9
                    }, {
                        Primitive = 5
                    }]
                }",
                @"{
                    ""MaybePocos"": [{
                        ""Primitive"": 9
                    },{
                        ""Primitive"": 5
                    }]
                }");
            AddSample(
                new NullableClassArray(new[] { null, new PrimitiveValue(5) }),
                "97cbdaaf66024eec0d0abd156c3d8076fc6b602966835a2c7682c05b407eef6a",
                @"{
                    MaybePocos = [null, {
                        Primitive = 5
                    }]
                }",
                @"{
                    ""MaybePocos"": [null,
                    {
                        ""Primitive"": 5
                    }]
                }");
        }
    }
}
