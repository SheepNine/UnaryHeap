using GeneratedTestPocos;
using NUnit.Framework;

namespace Pocotheosis.Tests.Values
{
    internal class PolymorphicValueTests : PocoTestFixture<PolymorphicValue>
    {
        public PolymorphicValueTests()
        {
            AddSample(
                new PolymorphicValue(P(1)),
                "38b6e69557b6b4b45d62e86ca4a960aa9849be4307204d3c5bfeb08bad033be5",
                @"{
                    Rainbow = {
                        Primitive = 1
                    }
                }",
                @"{
                    ""Rainbow"": {
                        ""type"": ""PrimitiveValue"",
                        ""data"": {
                            ""Primitive"": 1
                        }
                    }
                }");

            AddSample(
                new PolymorphicValue(new EnumValue(FNF)),
                "84258326622c5c9d19a94cb39e88ea52ab3b78e2808e7cc65cd5084fa92b62ef",
                @"{
                    Rainbow = {
                        Enum = FileNotFound
                    }
                }",
                @"{
                    ""Rainbow"": {
                        ""type"": ""EnumValue"",
                        ""data"": {
                            ""Enum"": ""FileNotFound""
                        }
                    }
                }");

            AddSample(
                new PolymorphicValue(new StringValue("seven")),
                "bf287385ec44ce5aab445b87211afc719cca124686bad228562a8614010c9334",
                @"{
                    Rainbow = {
                        Str = 'seven'
                    }
                }",
                @"{
                    ""Rainbow"": {
                        ""type"": ""StringValue"",
                        ""data"": {
                            ""Str"": ""seven""
                        }
                    }
                }");

            AddSample(
                new PolymorphicValue(new NullableStringValue(null)),
                "75e05fbb40bddfda59a28ee21316d675846c31876cbb7640950e654ad7c84536",
                @"{
                    Rainbow = {
                        MaybeString = null
                    }
                }",
                @"{
                    ""Rainbow"": {
                        ""type"": ""NullableStringValue"",
                        ""data"": {
                            ""MaybeString"": null
                        }
                    }
                }");

            AddSample(
                new PolymorphicValue(new ClassValue(P(1))),
                "c55b454471b495b8610f26871301836e4b3dfbc40ad1eb1fb3c4464ce2a7def1",
                @"{
                    Rainbow = {
                        Poco = {
                            Primitive = 1
                        }
                    }
                }",
                @"{
                    ""Rainbow"": {
                        ""type"": ""ClassValue"",
                        ""data"": {
                            ""Poco"": {
                                ""Primitive"": 1
                            }
                        }
                    }
                }");

            AddSample(
                new PolymorphicValue(new NullableClassValue(null)),
                "b496b4c41f88f61d70a82e98a264b85604bd84a80e9d99b91d47e0ee7b0f6c5f",
                @"{
                    Rainbow = {
                        MaybePoco = null
                    }
                }",
                @"{
                    ""Rainbow"": {
                        ""type"": ""NullableClassValue"",
                        ""data"": {
                            ""MaybePoco"": null
                        }
                    }
                }");

            AddSample(
                new PolymorphicValue(new PolymorphicValue(P(2))),
                "a81ddcb2ede75f409309fe0b76d6d91a83d8cc4d4c131696675226b6bded5fa1",
                @"{
                    Rainbow = {
                        Rainbow = {
                            Primitive = 2
                        }
                    }
                }",
                @"{
                    ""Rainbow"": {
                        ""type"": ""PolymorphicValue"",
                        ""data"": {
                            ""Rainbow"": {
                                ""type"": ""PrimitiveValue"",
                                ""data"": {
                                    ""Primitive"": 2
                                }
                            }
                        }
                    }
                }");

            AddInvalidConstructions(
                () => { var a = new PolymorphicValue(null); }
                );
        }

        [Test]
        public override void Builder()
        {
            var sut = new PolymorphicValue(P(8)).ToBuilder();
            Assert.AreEqual(P(8), sut.Rainbow);
            Assert.AreEqual(
                sut.WithRainbow(new PrimitiveValue(11)).Build(),
                new PolymorphicValue.Builder(new PrimitiveValue(11)).Build());
        }
    }
}
