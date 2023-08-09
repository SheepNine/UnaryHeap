using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System.Linq;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class ClassArrayPocoTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0, new ClassArrayPoco(new ScoreTuple[] { }).Scores.Count);
            var data = new ScoreTuple[]
            {
                new ScoreTuple("Alice", 872),
                new ScoreTuple("Bob", 1)
            };
            var poco = new ClassArrayPoco(data);
            Assert.AreEqual(2, poco.Scores.Count);
            data[0] = new ScoreTuple("Charles", -3); // Ensures poco made a copy
            Assert.AreEqual("Alice", poco.Scores[0].Name);
            Assert.AreEqual(872, poco.Scores[0].Score);
            Assert.AreEqual("Bob", poco.Scores[1].Name);
            Assert.AreEqual(1, poco.Scores[1].Score);
        }

        [Test]
        public void Constructor_NullReference()
        {
            Assert.Throws<System.ArgumentNullException>(() => new ClassArrayPoco(null));
            Assert.Throws<System.ArgumentNullException>(() => new ClassArrayPoco(
                new ScoreTuple[] { null }));
        }

        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new ClassArrayPoco(new[] { new ScoreTuple("Alice", 872) }));
            Assert.AreEqual(new ClassArrayPoco(new[] { new ScoreTuple("Alice", 872) }),
                new ClassArrayPoco(new[] { new ScoreTuple("Alice", 872) }));
            Assert.AreNotEqual(new ClassArrayPoco(new[] { new ScoreTuple("Alice", 872) }),
                new ClassArrayPoco(new[] { new ScoreTuple("Bob", 872) }));
            Assert.AreNotEqual(new ClassArrayPoco(new[] { new ScoreTuple("Alice", 872) }),
                new ClassArrayPoco(new[] { new ScoreTuple("Alice", 1) }));
            Assert.AreNotEqual(new ClassArrayPoco(new[] { new ScoreTuple("Alice", 872) }),
                new ClassArrayPoco(new ScoreTuple[] { }));
        }

        [Test]
        public void StringFormat()
        {
            Assert.AreEqual(
                "{\r\n\tScores = [{\r\n\t\tName = 'Solo'\r\n\t\tScore = 1\r\n\t}]\r\n}",
                new ClassArrayPoco(new ScoreTuple[] {
                    new ScoreTuple("Solo", 1)
                }).ToString());
            Assert.AreEqual(
                "{\r\n\tScores = [{\r\n\t\tName = 'Alice'\r\n\t\tScore = 77\r\n\t}, " +
                "{\r\n\t\tName = 'Bob'\r\n\t\tScore = 80\r\n\t}]\r\n}",
                new ClassArrayPoco(new ScoreTuple[] {
                    new ScoreTuple("Alice", 77),
                    new ScoreTuple("Bob", 80),
                }).ToString());
        }

        [Test]
        public void RoundTrip()
        {
            var data = new ScoreTuple[]
            {
                new ScoreTuple("Alice", 872),
                new ScoreTuple("Bob", 1)
            };
            TestUtils.TestRoundTrip(new ClassArrayPoco(data.Take(0)));
            TestUtils.TestRoundTrip(new ClassArrayPoco(data.Take(1)));
            TestUtils.TestRoundTrip(new ClassArrayPoco(data.Take(2)));
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<ClassArrayPoco>(@"{""Scores"":[]}");
            TestUtils.TestJsonRoundTrip<ClassArrayPoco>(
                @"{""Scores"":[{""Name"":""Alice"",""Score"":100}]}");
        }
    }
}
