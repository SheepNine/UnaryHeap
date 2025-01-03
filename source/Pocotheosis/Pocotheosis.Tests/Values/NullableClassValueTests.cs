﻿using NUnit.Framework;
using GeneratedTestPocos;

namespace Pocotheosis.Tests.Values
{
    public class NullableClassValueTests : PocoTestFixture<NullableClassValue>
    {
        public NullableClassValueTests()
        {
            AddSample(
                new NullableClassValue(new PrimitiveValue(1)),
                "5fc3b5971bfeac55e26b6e1136c322fa9b27c250fd59f1bbb3f89a3242923094",
                @"{
                    MaybePoco = {
                        Primitive = 1
                    }
                }",
                @"{
                    ""MaybePoco"": {
                        ""Primitive"": 1
                    }
                }");
            AddSample(
                new NullableClassValue(new PrimitiveValue(20)),
                "3207e2a967a57bc68c9ec6b66e58c236819934739dfd5b0257e37b3e736a08dd",
                @"{
                    MaybePoco = {
                        Primitive = 20
                    }
                }",
                @"{
                    ""MaybePoco"": {
                        ""Primitive"": 20
                    }
                }");
            AddSample(
                new NullableClassValue(null),
                "d524200a8bb9ca9a3e499f50f81d17b454f8e65c22479419106627446edd9a51",
                @"{
                    MaybePoco = null
                }",
                @"{
                    ""MaybePoco"": null
                }");

            NoInvalidConstructions();
        }

        [Test]
        public override void Builder()
        {
            var sut = new NullableClassValue(new PrimitiveValue(8)).ToBuilder();
            Assert.AreEqual(8, sut.MaybePoco.Primitive);
            Assert.AreEqual(
                sut.WithMaybePoco(new PrimitiveValue(11)).Build(),
                new NullableClassValue.Builder(new PrimitiveValue(11)).Build());
            Assert.AreEqual(
                sut.WithMaybePoco(null).Build(),
                new NullableClassValue.Builder(null).Build());
        }
    }
}
