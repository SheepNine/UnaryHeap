using NUnit.Framework;
using GeneratedTestPocos;
using System.Linq;
using Dataset = System.Collections.Generic.Dictionary
        <int, GeneratedTestPocos.IPoco>;
using KV = System.Collections.Generic.KeyValuePair
        <int, GeneratedTestPocos.IPoco>;

namespace Pocotheosis.Tests.Maps
{
    internal class PolymorphicMapTests : PocoTestFixture<PolymorphicMap>
    {
        public PolymorphicMapTests()
        {
            AddSample(
                new PolymorphicMap(new Dataset()),
                "1ba3a5513c4010ae40ac6bc0f7e30dd875ffe142ff3f785a77d42e3963b06b8c",
                @"{
                    Rainbows = ()
                }",
                @"{
                    ""Rainbows"": []
                }");
            AddSample(
                new PolymorphicMap(new Dataset() { { 7, P(25) } }),
                "977d31fea4b37013cb439dd335ae844f8e7f8a50041f9ec6e100a9f64df89a14",
                @"{
                    Rainbows = (
                        7 -> {
                            Primitive = 25
                        }
                    )
                }",
                @"{
                    ""Rainbows"": [{
                        ""k"": 7,
                        ""v"": {
                            ""type"": ""PrimitiveValue"",
                            ""data"": {
                                ""Primitive"": 25
                            }
                        }
                    }]
                }");
            AddSample(
                new PolymorphicMap(new Dataset() {
                    { 12, P(2) },
                    { 15, new EnumValue(Tru) }
                }),
                "11dad90b8bd2f6789877964112f6a7c92036de2ec87f521222e890efc7a4fed3",
                @"{
                    Rainbows = (
                        12 -> {
                            Primitive = 2
                        },
                        15 -> {
                            Enum = True
                        }
                    )
                }",
                @"{
                    ""Rainbows"": [{
                        ""k"": 12,
                        ""v"": {
                            ""type"": ""PrimitiveValue"",
                            ""data"": {
                                ""Primitive"": 2
                            }
                        }
                    },{
                        ""k"": 15,
                        ""v"": {
                            ""type"": ""EnumValue"",
                            ""data"": {
                                ""Enum"": ""True""
                            }
                        }
                    }]
                }");

            AddInvalidConstructions(
                () => { var a = new PolymorphicMap(null); },
                () => { var a = new PolymorphicMap.Builder(null); },
                () => { var a = new PolymorphicMap(new Dataset() { { 7, null } }); },
                () => { var a = new PolymorphicMap.Builder(new Dataset() { { 19, null } }); },
                () => { new PolymorphicMap.Builder(new Dataset()).SetRainbow(65, null); }
            );
        }

        [Test]
        public override void Builder()
        {
            var Ka = 7;
            var Kb = 14;
            var Kc = 21;
            var Kd = 28;
            var Va = P(5);
            var Vb = new EnumValue(Tru);
            var Vc = P(99);
            var Vd = P(2);

            var sut = new PolymorphicMap(new Dataset() { { Ka, Va }, { Kb, Vb } }).ToBuilder();
            sut.SetRainbow(Ka, Vc);
            sut.RemoveRainbow(Kb);
            Assert.True(sut.ContainsRainbowKey(Ka));
            Assert.False(sut.ContainsRainbowKey(Kb));
            Assert.AreEqual(1, sut.CountRainbows);
            Assert.AreEqual(Vc, sut.GetRainbow(Ka));
            Assert.AreEqual(new[] { Ka }, sut.RainbowKeys);
            Assert.AreEqual(new[] { new KV(Ka, Vc) },
                sut.RainbowValues.Select(kv => new KV(kv.Key, kv.Value)));

            sut.ClearRainbows();
            Assert.AreEqual(0, sut.CountRainbows);

            sut.SetRainbow(Kc, Vc);
            sut.SetRainbow(Kd, Vd);
            Assert.AreEqual(
                new PolymorphicMap.Builder(new Dataset() { { Kc, Vc }, { Kd, Vd } }).Build(),
                sut.Build());
        }
    }
}
