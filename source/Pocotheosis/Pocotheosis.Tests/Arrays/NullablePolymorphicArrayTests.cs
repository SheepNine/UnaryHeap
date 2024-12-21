using NUnit.Framework;
using GeneratedTestPocos;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    public class NullablePolymorphicArrayTests : PocoTestFixture<NullablePolymorphicArray>
    {
        static NullablePolymorphicArray.Builder EmptyBuilder
        {
            get { return new NullablePolymorphicArray.Builder(Enumerable.Empty<IPoco>()); }
        }

        public NullablePolymorphicArrayTests()
        {
            AddSample(
                new NullablePolymorphicArray(Enumerable.Empty<PrimitiveValue>()),
                "9c16f37399c5ead6c47e0d33ed6ec627132c3d130872217badc2ce4a1198a582",
                @"{
                    MaybeRainbows = []
                }",
                @"{
                    ""MaybeRainbows"": []
                }");
            AddSample(
                new NullablePolymorphicArray(new[] { P(88) }),
                "4fa8ea6b17b74657b70c6426040d8b51b395d23626e26f47848a4e6c6f5809b6",
                @"{
                    MaybeRainbows = [{
                        Primitive = 88
                    }]
                }",
                @"{
                    ""MaybeRainbows"": [{
                        ""type"": ""PrimitiveValue"",
                        ""data"": {
                            ""Primitive"": 88
                        }
                    }]
                }");
            AddSample(
                new NullablePolymorphicArray(new PrimitiveValue[] { null }),
                "6883f7618ac15773c96bc08303339aac1b0b8cfd3b2c770ee8b252718071ae53",
                @"{
                    MaybeRainbows = [null]
                }",
                @"{
                    ""MaybeRainbows"": [null]
                }");
            AddSample(
                new NullablePolymorphicArray(new IPoco[] { new EnumValue(FNF), P(5) }),
                "7a58bd5347a6269630e13475da0262a68244c54d726f6c2fb053e47e1aa08b84",
                @"{
                    MaybeRainbows = [{
                        Enum = FileNotFound
                    }, {
                        Primitive = 5
                    }]
                }",
                @"{
                    ""MaybeRainbows"": [{
                        ""type"": ""EnumValue"",
                        ""data"": {
                            ""Enum"": ""FileNotFound""
                        }
                    },{
                        ""type"": ""PrimitiveValue"",
                        ""data"": {
                            ""Primitive"": 5
                        }
                    }]
                }");
            AddSample(
                new NullablePolymorphicArray(new[] { null, P(5) }),
                "68aa179acfac09c403fbc2570122b38531101ababd7d1f1ed2e70089f87ed811",
                @"{
                    MaybeRainbows = [null, {
                        Primitive = 5
                    }]
                }",
                @"{
                    ""MaybeRainbows"": [null,
                    {
                        ""type"": ""PrimitiveValue"",
                        ""data"": {
                            ""Primitive"": 5
                        }
                    }]
                }");

            AddInvalidConstructions(
                () => { var a = new NullablePolymorphicArray(null); },
                () => { var a = new NullablePolymorphicArray.Builder(null); },
                () => { EmptyBuilder.WithMaybeRainbows(null); }
            );
        }

        [Test]
        public override void Builder()
        {
            var A = null as PrimitiveValue;
            var B = P(3);
            var C = new EnumValue(Tru);

            var sut = new NullablePolymorphicArray(new IPoco[] { A, B, C }).ToBuilder();
            sut.SetMaybeRainbow(0, B);
            sut.SetMaybeRainbow(2, A);
            sut.RemoveMaybeRainbowAt(1);
            Assert.AreEqual(2, sut.NumMaybeRainbows);
            Assert.AreEqual(A, sut.GetMaybeRainbow(1));
            Assert.AreEqual(new[] { B, A }, sut.MaybeRainbowValues);

            sut.ClearMaybeRainbows();
            sut.AppendMaybeRainbow(B);
            sut.AppendMaybeRainbow(A);
            sut.InsertMaybeRainbowAt(0, C);
            sut.InsertMaybeRainbowAt(2, A);
            Assert.AreEqual(
                EmptyBuilder.WithMaybeRainbows(new IPoco[] { C, B, A, A }).Build(),
                sut.Build());
        }
    }
}
