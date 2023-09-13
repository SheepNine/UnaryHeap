using NUnit.Framework;
using GeneratedTestPocos;
using Dataset = System.Collections.Generic.Dictionary<bool, byte>;
using KV = System.Collections.Generic.KeyValuePair<bool, byte>;
using System.Linq;

namespace Pocotheosis.Tests.Maps
{
    internal class PrimitiveMapTests : PocoTestFixture<PrimitiveMap>
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
                () => { var a = new PrimitiveMap(null); },
                () => { var a = new PrimitiveMap.Builder(null); },
                () => { ReadTypedFromJson("{\"Primitives\":[{\"foo\":1}]}", false); },
                () => { ReadTypedFromJson("{\"Primitives\":{}}", false); }
            );
        }

        [Test]
        public void CollectionWrapper()
        {
            var sut = new PrimitiveMap(new Dataset() { { true, 24 } }).Primitives;
            Assert.AreEqual(24, sut[true]);
            Assert.AreEqual(new[] { true }, sut.Keys);
            Assert.AreEqual(new byte[] { 24 }, sut.Values);
            Assert.IsTrue(sut.ContainsKey(true));
            Assert.IsFalse(sut.ContainsKey(false));
            Assert.IsFalse(sut.TryGetValue(false, out _));
            Assert.IsTrue(sut.TryGetValue(true, out byte valueRead));
            Assert.AreEqual(24, valueRead);
            Assert.AreEqual(new[] { new KV(true, 24) }, sut.ToArray());
            Assert.AreEqual(new[] { new KV(true, 24) }, sut.OfType<KV>().ToArray());
        }

        [Test]
        public override void Builder()
        {
            var Ka = true;
            var Kb = false;
            var Kc = true;
            var Kd = false;
            var Va = (byte)8;
            var Vb = (byte)2;
            var Vc = (byte)16;
            var Vd = (byte)99;

            var sut = new PrimitiveMap(new Dataset() { { Ka, Va }, { Kb, Vb } }).ToBuilder();
            sut.SetPrimitive(Ka, Vc);
            sut.RemovePrimitive(Kb);
            Assert.True(sut.ContainsPrimitiveKey(Ka));
            Assert.False(sut.ContainsPrimitiveKey(Kb));
            Assert.AreEqual(1, sut.CountPrimitives);
            Assert.AreEqual(Vc, sut.GetPrimitive(Ka));
            Assert.AreEqual(new[] { Ka }, sut.PrimitiveKeys);
            Assert.AreEqual(new[] { new KV(Ka, Vc) }, sut.PrimitiveValues);

            sut.ClearPrimitives();
            Assert.AreEqual(0, sut.CountPrimitives);

            sut.SetPrimitive(Kc, Vc);
            sut.SetPrimitive(Kd, Vd);
            Assert.AreEqual(
                new PrimitiveMap.Builder(new Dataset() { { Kc, Vc }, { Kd, Vd } }).Build(),
                sut.Build());
        }
    }
}
