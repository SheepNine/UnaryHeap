using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class EnumPocoTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(TestEnum.False, new EnumPoco(TestEnum.False).Albedo);
            Assert.AreEqual(TestEnum.FileNotFound, new EnumPoco(TestEnum.FileNotFound).Albedo);
        }

        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new EnumPoco(TestEnum.False));
            Assert.AreEqual(new EnumPoco(TestEnum.False), new EnumPoco(TestEnum.False));
            Assert.AreNotEqual(new EnumPoco(TestEnum.FileNotFound), new EnumPoco(TestEnum.False));
        }

        [Test]
        public void StringFormat()
        {
            Assert.AreEqual("EnumPoco\r\n\tAlbedo: False", new EnumPoco(TestEnum.False).ToString());
            Assert.AreEqual("EnumPoco\r\n\tAlbedo: FileNotFound", new EnumPoco(TestEnum.FileNotFound).ToString());
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new EnumPoco(TestEnum.False));
            TestUtils.TestRoundTrip(new EnumPoco(TestEnum.FileNotFound));
        }
    }
}
