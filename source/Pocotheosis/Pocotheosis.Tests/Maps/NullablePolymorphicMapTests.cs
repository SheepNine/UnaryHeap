using NUnit.Framework;
using GeneratedTestPocos;
using Dataset = System.Collections.Generic.Dictionary
    <int, GeneratedTestPocos.IPoco>;
using KV = System.Collections.Generic.KeyValuePair
    <int, GeneratedTestPocos.IPoco>;

namespace Pocotheosis.Tests.Maps
{
    internal class NullablePolymorphicMapTests : PocoTestFixture<NullablePolymorphicMap>
    {
        public NullablePolymorphicMapTests()
        {
            AddSample(
                new NullablePolymorphicMap(new Dataset()),
                "900374d9cbbf2bacc2df30f3468be675f883a7feea3e2e2ca709c71b3b31a586",
                @"{
                    MaybeRainbows = ()
                }",
                @"{
                    ""MaybeRainbows"": []
                }");
            AddSample(
                new NullablePolymorphicMap(new Dataset() { { 33, P(25) } }),
                "c66a744ac11930af04a4327290c49af60861dead87f9cb9220b50aad4220d115",
                @"{
                    MaybeRainbows = (
                        33 -> {
                            Primitive = 25
                        }
                    )
                }",
                @"{
                    ""MaybeRainbows"": [{
                        ""k"": 33,
                        ""v"": {
                            ""type"": ""PrimitiveValue"",
                            ""data"": {
                                ""Primitive"": 25
                            }
                        }
                    }]
                }");
            AddSample(
                new NullablePolymorphicMap(new Dataset() { { 75, null } }),
                "bb0215347c02c91655ebc18371cc638c743ff7c4f36e9db432112d5e482921f8",
                @"{
                    MaybeRainbows = (
                        75 -> null
                    )
                }",
                @"{
                    ""MaybeRainbows"": [{
                        ""k"": 75,
                        ""v"": null
                    }]
                }");
            AddSample(
                new NullablePolymorphicMap(new Dataset() {
                    { 143, P(2) },
                    { 64, new EnumValue(Tru) }
                }),
                "baaac6cefedcccc75946a6d1f5227eb8e41f49749f23cbff7a6b6cf0a951c679",
                @"{
                    MaybeRainbows = (
                        64 -> {
                            Enum = True
                        },
                        143 -> {
                            Primitive = 2
                        }
                    )
                }",
                @"{
                    ""MaybeRainbows"": [{
                        ""k"": 64,
                        ""v"": {
                            ""type"": ""EnumValue"",
                            ""data"": {
                                ""Enum"": ""True""
                            }
                        }
                    },{
                        ""k"": 143,
                        ""v"": {
                            ""type"": ""PrimitiveValue"",
                            ""data"": {
                                ""Primitive"": 2
                            }
                        }
                    }]
                }");
            AddSample(
                new NullablePolymorphicMap(new Dataset() {
                    { 43, null },
                    { 87, P(101) }
                }),
                "18e48b72261b8741fbe46b25969a217cb7c226bce7899d0659ca6cf03d692b89",
                @"{
                    MaybeRainbows = (
                        43 -> null,
                        87 -> {
                            Primitive = 101
                        }
                    )
                }",
                @"{
                    ""MaybeRainbows"": [{
                        ""k"": 43,
                        ""v"": null
                    },{
                        ""k"": 87,
                        ""v"": {
                            ""type"": ""PrimitiveValue"",
                            ""data"": {
                                ""Primitive"": 101
                            }
                        }
                    }]
                }");

            AddInvalidConstructions(
                () => { var a = new NullablePolymorphicMap(null); },
                () => { var a = new NullablePolymorphicMap.Builder(null); }
            );
        }

        [Test]
        public override void Builder()
        {
            var Ka = 4;
            var Kb = 3;
            var Kc = 2;
            var Kd = 1;
            var Va = P(5);
            var Vb = new EnumValue(FNF);
            var Vc = (PrimitiveValue)null;
            var Vd = P(2);

            var sut = new NullablePolymorphicMap(
                new Dataset() { { Ka, Va }, { Kb, Vb } }).ToBuilder();
            sut.SetMaybeRainbow(Ka, Vc);
            sut.RemoveMaybeRainbow(Kb);
            Assert.True(sut.ContainsMaybeRainbowKey(Ka));
            Assert.False(sut.ContainsMaybeRainbowKey(Kb));
            Assert.AreEqual(1, sut.CountMaybeRainbows);
            Assert.AreEqual(Vc, sut.GetMaybeRainbow(Ka));
            Assert.AreEqual(new[] { Ka }, sut.MaybeRainbowKeys);
            Assert.AreEqual(new[] { new KV(Ka, Vc) }, sut.MaybeRainbowValues);

            sut.ClearMaybeRainbows();
            Assert.AreEqual(0, sut.CountMaybeRainbows);

            sut.SetMaybeRainbow(Kc, Vc);
            sut.SetMaybeRainbow(Kd, Vd);
            Assert.AreEqual(
                new NullablePolymorphicMap.Builder(
                    new Dataset() { { Kc, Vc }, { Kd, Vd } }).Build(),
                sut.Build());
        }
    }
}
