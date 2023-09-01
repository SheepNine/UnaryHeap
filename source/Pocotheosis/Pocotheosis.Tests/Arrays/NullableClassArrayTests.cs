using NUnit.Framework;
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
                new NullableClassArray(new[] { P(88) }),
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
                new NullableClassArray(new[] { P(9), P(5) }),
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
                new NullableClassArray(new[] { null, P(5) }),
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

            AddInvalidConstructions(
                () => { var a = new NullableClassArray(null); },
                () => { var a = new NullableClassArray.Builder(null); }
            );
        }

        [Test]
        public override void Builder()
        {
            var A = null as PrimitiveValue;
            var B = P(3);
            var C = P(5);

            var sut = new NullableClassArray(new[] { A, B, C }).ToBuilder();
            sut.SetMaybePoco(2, A);
            sut.RemoveMaybePocoAt(1);
            Assert.AreEqual(2, sut.NumMaybePocos);
            Assert.AreEqual(A, sut.GetMaybePoco(1));
            Assert.AreEqual(new[] { A, A }, sut.MaybePocoValues);

            sut.ClearMaybePocos();
            sut.AppendMaybePoco(B);
            sut.InsertMaybePocoAt(0, C);
            sut.InsertMaybePocoAt(2, A);
            Assert.AreEqual(
                new NullableClassArray.Builder(new[] { C, B, A }).Build(),
                sut.Build());
        }
    }
}
