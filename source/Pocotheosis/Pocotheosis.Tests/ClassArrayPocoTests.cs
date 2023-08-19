using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Linq;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class ClassArrayPocoTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0, new ClassArrayPoco(Array.Empty<ScoreTuple>()).Scores.Count);
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
        public void Checksum()
        {
            TestUtils.TestChecksum(
                new ClassArrayPoco(new[] { new ScoreTuple("Alice", 3) }),
                "17d99d96b046e64cbe7680a90c725789fcc78d671f1bf929e9523dd18a3f76cc");
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(() => new ClassArrayPoco(null));
            Assert.Throws<ArgumentNullException>(() => new ClassArrayPoco(
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
                new ClassArrayPoco(Array.Empty<ScoreTuple>()));
        }

        [Test]
        public void StringFormat()
        {
            TestUtils.TestToString(new() { {
                new ClassArrayPoco(new ScoreTuple[] {
                    new ScoreTuple("Solo", 1)
                }),
                @"{
                    Scores = [{
                        Name = 'Solo'
                        Score = 1
                    }]
                }"
            }, {
                new ClassArrayPoco(new ScoreTuple[] {
                    new ScoreTuple("Alice", 77),
                    new ScoreTuple("Bob", 80),
                }),
                @"{
                    Scores = [{
                        Name = 'Alice'
                        Score = 77
                    }, {
                        Name = 'Bob'
                        Score = 80
                    }]
                }"
            } });
        }

        [Test]
        public void RoundTrip()
        {
            var data = new ScoreTuple[]
            {
                new ScoreTuple("Alice", 872),
                new ScoreTuple("Bob", 1)
            };

            TestUtils.TestRoundTrip(
                new ClassArrayPoco(data.Take(0)),
                new ClassArrayPoco(data.Take(1)),
                new ClassArrayPoco(data.Take(2))
            );
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<ClassArrayPoco>(@"{
                ""Scores"": []
            }", @"{
                ""Scores"": [{
                    ""Name"": ""Alice"",
                    ""Score"": 100
                }]
            }");
        }
    }
}
