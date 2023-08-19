using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class EmptyPocoTests
    {
        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new EmptyPoco());
            Assert.AreEqual(new EmptyPoco(), new EmptyPoco());
            Assert.AreNotEqual(new EmptyPoco(), new BoolPoco(false));
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new EmptyPoco(),
                "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new EmptyPoco(),
                "{ }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(new EmptyPoco());
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<EmptyPoco>("{}");
        }
    }
}
