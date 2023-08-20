using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;

namespace Pocotheosis.Tests.Values
{
    [TestFixture]
    public class ClassValueTests
    {
        static PrimitiveValue P(byte value) { return new PrimitiveValue(value); }


        [Test]
        public void Constructor()
        {
            var sut = new ClassValue(P(19));
            Assert.AreEqual(19, sut.Value.Value);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ClassValue(null));
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(new ClassValue(P(3)), new ClassValue(P(3)));
            Assert.AreNotEqual(new ClassValue(P(3)), new ClassValue(P(5)));
        }

        [Test]
        public void Builder()
        {
            var source = new ClassValue(P(0));
            var target = new ClassValue(P(60));

            {
                var sut = source.ToBuilder();
                sut.WithValue(P(60));
                Assert.AreEqual(sut.Build(), target);
            }
        }

        [Test]
        public void BuilderNullValue()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ClassValue.Builder(null));
            Assert.Throws<ArgumentNullException>(() =>
                new ClassValue.Builder(P(0)).WithValue(null));
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new ClassValue(P(105)),
                "de7d1b721a1e0632b7cf04edf5032c8ecffa9f9a08492152b926f1a5a7e765d7");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new ClassValue(P(20)),
                @"{
                    Value = {
                        Value = 20
                    }
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new ClassValue(P(6))
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<ClassValue>(@"{
                ""Value"": {
                    ""Value"": 127
                }
            }");
        }
    }
}
