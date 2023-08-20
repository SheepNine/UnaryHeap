using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Linq;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class ClassArrayPocoTests
    {
        static ScoreTuple Alice() { return new ScoreTuple("Alice", 1); }
        static ScoreTuple Bob() { return new ScoreTuple("Bob", 2); }
        static ScoreTuple Charlie() { return new ScoreTuple("Charlie", 3); }

        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0, new ClassArrayPoco(Array.Empty<ScoreTuple>()).Scores.Count);
            var data = new ScoreTuple[] { Alice(), null, Bob() };
            var poco = new ClassArrayPoco(data);
            Assert.AreEqual(3, poco.Scores.Count);
            data[0] = Charlie(); // Ensures poco made a copy
            Assert.AreEqual("Alice", poco.Scores[0].Name);
            Assert.AreEqual(1, poco.Scores[0].Score);
            Assert.IsNull(poco.Scores[1]);
            Assert.AreEqual("Bob", poco.Scores[2].Name);
            Assert.AreEqual(2, poco.Scores[2].Score);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(() => new ClassArrayPoco(null));
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(
                new ClassArrayPoco(new[] { Alice() }),
                new ClassArrayPoco(new[] { Alice() }));
            Assert.AreNotEqual(
                new ClassArrayPoco(new[] { Alice() }),
                new ClassArrayPoco(new[] { Bob() }));
            Assert.AreNotEqual(
                new ClassArrayPoco(new[] { Alice() }),
                new ClassArrayPoco(new[] { Alice(), Bob() }));
            Assert.AreNotEqual(
                new ClassArrayPoco(new[] { Alice() }),
                new ClassArrayPoco(Enumerable.Empty<ScoreTuple>()));
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new ClassArrayPoco(new[] { Alice(), null, Bob() }),
                "d8daef9a9cb7232292d6249e1e143df2030375ef347ce0ed25d9460b5f06b0d1");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
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
                    null,
                    new ScoreTuple("Bob", 80),
                }),
                @"{
                    Scores = [{
                        Name = 'Alice'
                        Score = 77
                    }, null, {
                        Name = 'Bob'
                        Score = 80
                    }]
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            var data = new ScoreTuple[]
            {
                Alice(),
                null,
                Bob(),
                Charlie()
            };

            PocoTest.Serialization(
                new ClassArrayPoco(data.Take(0)),
                new ClassArrayPoco(data.Take(1)),
                new ClassArrayPoco(data.Take(2)),
                new ClassArrayPoco(data.Take(3))
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<ClassArrayPoco>(@"{
                ""Scores"": []
            }", @"{
                ""Scores"": [{
                    ""Name"": ""Charlie"",
                    ""Score"": 3
                }]
            }", @"{
                ""Scores"": [{
                    ""Name"": ""Alice"",
                    ""Score"": 1
                },
                null,
                {
                    ""Name"": ""Bob"",
                    ""Score"": 2
                }]
            }");
        }
    }
}
