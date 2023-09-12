using GeneratedTestPocos;
using NUnit.Framework;

namespace Pocotheosis.Tests.Values
{
    internal class NullablePolymorphicValueTests : PocoTestFixture<NullablePolymorphicValue>
    {
        public NullablePolymorphicValueTests()
        {
            AddSample(new NullablePolymorphicValue(null),
                "548a7544e4b41d37e5170afe38c51c7fe14811217a1f00137b86cd8306b44cdd",
                @"{
                    MaybeRainbow = null
                }",
                @"{
                    ""MaybeRainbow"": null
                }");

            AddSample(
                new NullablePolymorphicValue(P(11)),
                "7ea2414f6135765ec09309429e433020f89a217671fca2a664efca0fc11f3f3f",
                @"{
                    MaybeRainbow = {
                        Primitive = 11
                    }
                }",
                @"{
                    ""MaybeRainbow"": {
                        ""type"": ""PrimitiveValue"",
                        ""data"": {
                            ""Primitive"": 11
                        }
                    }
                }");

            AddSample(
                new NullablePolymorphicValue(new EnumValue(FNF)),
                "c4ee82b29ab4e5b35c31338993814f12b7e4a22d15b0de03458effd667b19db3",
                @"{
                    MaybeRainbow = {
                        Enum = FileNotFound
                    }
                }",
                @"{
                    ""MaybeRainbow"": {
                        ""type"": ""EnumValue"",
                        ""data"": {
                            ""Enum"": ""FileNotFound""
                        }
                    }
                }");

            AddSample(
                new NullablePolymorphicValue(new StringValue("eleven")),
                "bee59a16d120f5d420c065bb50a95cfb004d924a5fa5cafffcffc493f60cc506",
                @"{
                    MaybeRainbow = {
                        Str = 'eleven'
                    }
                }",
                @"{
                    ""MaybeRainbow"": {
                        ""type"": ""StringValue"",
                        ""data"": {
                            ""Str"": ""eleven""
                        }
                    }
                }");

            NoInvalidConstructions();
        }

        [Test]
        public override void Builder()
        {
            var sut = new NullablePolymorphicValue(P(8)).ToBuilder();
            Assert.AreEqual(P(8), sut.MaybeRainbow);
            Assert.AreEqual(
                sut.WithMaybeRainbow(P(11)).Build(),
                new NullablePolymorphicValue.Builder(P(11)).Build());
            Assert.AreEqual(
                sut.WithMaybeRainbow(null).Build(),
                new NullablePolymorphicValue.Builder(null).Build());
        }
    }
}
