using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class ClassPocoTests
    {
        [Test]
        public void Constructor()
        {
            var sut = new ClassPoco(new ScoreTuple("Bob", 1));
            Assert.AreEqual("Bob", sut.Score.Name);
            Assert.AreEqual(1, sut.Score.Score);

            sut = new ClassPoco(new ScoreTuple("Alice", 872));
            Assert.AreEqual("Alice", sut.Score.Name);
            Assert.AreEqual(872, sut.Score.Score);
        }

        [Test]
        public void Constructor_NullReference()
        {
            Assert.Throws<System.ArgumentNullException>(() => new ClassPoco(null));
        }

        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new ClassPoco(new ScoreTuple("Bob", 1)));
            Assert.AreEqual(new ClassPoco(new ScoreTuple("Bob", 1)),
                new ClassPoco(new ScoreTuple("Bob", 1)));
            Assert.AreNotEqual(new ClassPoco(new ScoreTuple("Alice", 1)),
                new ClassPoco(new ScoreTuple("Bob", 1)));
            Assert.AreNotEqual(new ClassPoco(new ScoreTuple("Bob", 4)),
                new ClassPoco(new ScoreTuple("Bob", 1)));
        }

        [Test]
        [Ignore("NEEDS WORK")]
        public void StringFormat()
        {
            Assert.AreEqual("ClassPoco\r\n\tScore:\r\n\t\tName: Alice\r\n\t\tScore: 872",
                new ClassPoco(new ScoreTuple("Alice", 872)).ToString());
            Assert.AreEqual("ClassPoco\r\n\tScore:\r\n\t\tName: Bob\r\n\t\tScore: 1",
                new ClassPoco(new ScoreTuple("Bob", 1)).ToString());
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new ClassPoco(new ScoreTuple("Alice", 872)));
            TestUtils.TestRoundTrip(new ClassPoco(new ScoreTuple("Bob", 1)));
        }
    }
}
