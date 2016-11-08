using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System.Linq;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class EnumArrayPocoTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0, new EnumArrayPoco(new TestEnum[0]).Nigredo.Count);
            var data = new TestEnum[] { TestEnum.False, TestEnum.True };
            var poco = new EnumArrayPoco(data);
            Assert.AreEqual(2, poco.Nigredo.Count);
            data[0] = TestEnum.FileNotFound; // Ensures poco made a copy
            Assert.AreEqual(TestEnum.False, poco.Nigredo[0]);
            Assert.AreEqual(TestEnum.True, poco.Nigredo[1]);
            Assert.True(poco.Nigredo.IsReadOnly);
        }

        [Test]
        public void Constructor_NullReference()
        {
            Assert.Throws<System.ArgumentNullException>(() => new EnumArrayPoco(null));
        }

        [Test]
        public void Equality()
        {
            var data = new[] { TestEnum.True, TestEnum.False, TestEnum.FileNotFound };

            Assert.AreNotEqual(null, new EnumArrayPoco(data.Take(2)));
            Assert.AreEqual(new EnumArrayPoco(data.Take(2)),
                new EnumArrayPoco(data.Take(2)));
            Assert.AreNotEqual(new EnumArrayPoco(data.Take(1)),
                new EnumArrayPoco(data.Take(2)));
            Assert.AreNotEqual(new EnumArrayPoco(data.Take(3)),
                new EnumArrayPoco(data.Take(2)));
            Assert.AreNotEqual(new EnumArrayPoco(data),
                new EnumArrayPoco(data.Reverse()));
        }

        [Test]
        public void StringFormat()
        {
            Assert.AreEqual("EnumArrayPoco\r\n\tNigredo: <empty>",
                new EnumArrayPoco(new TestEnum[] { }).ToString());
            Assert.AreEqual("EnumArrayPoco\r\n\tNigredo: False",
                new EnumArrayPoco(new TestEnum[] { TestEnum.False }).ToString());
            Assert.AreEqual("EnumArrayPoco\r\n\tNigredo: False, True",
                new EnumArrayPoco(new TestEnum[] { TestEnum.False, TestEnum.True }).ToString());
        }

        [Test]
        public void RoundTrip()
        {
            var data = new[] { TestEnum.True, TestEnum.False, TestEnum.FileNotFound };
            TestUtils.TestRoundTrip(new EnumArrayPoco(data.Take(0)));
            TestUtils.TestRoundTrip(new EnumArrayPoco(data.Take(1)));
            TestUtils.TestRoundTrip(new EnumArrayPoco(data.Take(2)));
        }
    }
}
