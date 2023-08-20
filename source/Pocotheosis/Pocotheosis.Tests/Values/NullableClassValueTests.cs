using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    [TestFixture]
    public class NullableClassValueTests
    {
        static PrimitiveValue P(byte value) { return new PrimitiveValue(value); }


        [Test]
        public void Constructor()
        {
            var sut = new NullableClassValue(P(20));
            Assert.AreEqual(20, sut.MaybePoco.Primitive);
            sut = new NullableClassValue(null);
            Assert.IsNull(sut.MaybePoco);
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(new NullableClassValue(P(3)), new NullableClassValue(P(3)));
            Assert.AreNotEqual(new NullableClassValue(P(3)), new NullableClassValue(P(5)));
            Assert.AreNotEqual(new NullableClassValue(P(3)), new NullableClassValue(null));
        }

        [Test]
        public void Builder()
        {
            var source = new NullableClassValue(P(0));
            var target = new NullableClassValue(null);

            {
                var sut = source.ToBuilder();
                sut.WithMaybePoco(null);
                Assert.AreEqual(sut.Build(), target);
            }
            {
                var sut = target.ToBuilder();
                sut.WithMaybePoco(P(0));
                Assert.AreEqual(sut.Build(), source);
            }
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new NullableClassValue(P(18)),
                "d9f932564043005f81dcc96185292f0399ee911c44b15ddfb5ecd00ed5259c1c");
            PocoTest.Checksum(
                new NullableClassValue(null),
                "ad95131bc0b799c0b1af477fb14fcf26a6a9f76079e48bf090acb7e8367bfd0e");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new NullableClassValue(P(80)),
                @"{
                    MaybePoco = {
                        Primitive = 80
                    }
                }"
            }, {
                new NullableClassValue(null),
                @"{
                    MaybePoco = null
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new NullableClassValue(P(6)),
                new NullableClassValue(null)
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<NullableClassValue>(@"{
                ""MaybePoco"": {
                    ""Primitive"": 127
                }
            }", @"{
                ""MaybePoco"": null
            }");
        }
    }
}
