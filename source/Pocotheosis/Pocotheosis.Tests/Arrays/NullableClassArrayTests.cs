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
                "4d9c858d0a75e1a789834a9677185c7c428a3ce7ba97501c94020eae498ee183",
                @"{
                    MaybePocos = []
                }",
                @"{
                    ""MaybePocos"": []
                }");
            AddSample(
                new NullableClassArray(new[] { new PrimitiveValue(88) }),
                "99655bc8a13326638f1f92b273cf21b587f3ec0263445eacc6dacfcc64976c1f",
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
                "b9b57fb9fd9878b696d28b722f330efb46985b012349dab83c0068f6b1f67cf2",
                @"{
                    MaybePocos = [null]
                }",
                @"{
                    ""MaybePocos"": [null]
                }");
            AddSample(
                new NullableClassArray(new[] { new PrimitiveValue(9), new PrimitiveValue(5) }),
                "6898ecc48076b279dd4a2a9365a645894a32fc21c745bfc13f0ce870e99bf21b",
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
                "c40f25b7e6c84a02eb1c3d1caac4e0233de7227e9c86459acc499be884c85dfa",
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
