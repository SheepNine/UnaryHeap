using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class ClassPocoTests
    {
        Func<ScoreTuple> Alice = () => new ScoreTuple("Alice", 1);
        Func<ScoreTuple> Bob = () => new ScoreTuple("Bob", 2);
        Func<ScoreTuple> Charlie = () => new ScoreTuple("Charlie", 3);


        [Test]
        public void Constructor()
        {
            var sut = new ClassPoco(Alice(), Bob());
            Assert.AreEqual("Alice", sut.Score.Name);
            Assert.AreEqual(1, sut.Score.Score);
            Assert.AreEqual("Bob", sut.NullScore.Name);
            Assert.AreEqual(2, sut.NullScore.Score);

            sut = new ClassPoco(Charlie(), null);
            Assert.AreEqual("Charlie", sut.Score.Name);
            Assert.AreEqual(3, sut.Score.Score);
            Assert.IsNull(sut.NullScore);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ClassPoco(null, Alice()));
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(new ClassPoco(Alice(), Bob()), new ClassPoco(Alice(), Bob()));
            Assert.AreNotEqual(new ClassPoco(Alice(), Bob()), new ClassPoco(Alice(), null));
            Assert.AreNotEqual(new ClassPoco(Alice(), Bob()), new ClassPoco(Alice(), Charlie()));
            Assert.AreNotEqual(new ClassPoco(Alice(), Bob()), new ClassPoco(Bob(), Bob()));
            Assert.AreNotEqual(new ClassPoco(Alice(), Bob()), new ClassPoco(Charlie(), Bob()));
        }

        [Test]
        public void Builder()
        {
            {
                var sut = new ClassPoco(Alice(), Bob()).ToBuilder();
                sut.Score.Name = "Charlie";
                sut.Score.Score = 3;
                sut.WithNullScore(null);
                Assert.AreEqual(new ClassPoco(Charlie(), null), sut.Build());
            }
            {
                var sut = new ClassPoco(Charlie(), null).ToBuilder();
                sut.WithScore(Alice()).WithNullScore(Bob());
                Assert.AreEqual(new ClassPoco(Alice(), Bob()), sut.Build());
            }
        }

        [Test]
        public void BuilderNullValue()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ClassPoco.Builder(null, Alice()));
            Assert.Throws<ArgumentNullException>(() =>
                new ClassPoco.Builder(Alice(), null).WithScore(null));
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new ClassPoco(Alice(), null),
                "64ba51ac3d22d796519b6bdabf4b4644c80d04ed7bc37eb8362a528ba568846f");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new ClassPoco(Alice(), Bob()),
                @"{
                    Score = {
                        Name = 'Alice'
                        Score = 1
                    }
                    NullScore = {
                        Name = 'Bob'
                        Score = 2
                    }
                }"
            }, {
                new ClassPoco(Charlie(), null),
                @"{
                    Score = {
                        Name = 'Charlie'
                        Score = 3
                    }
                    NullScore = null
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new ClassPoco(Alice(), Bob()),
                new ClassPoco(Charlie(), null)
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<ClassPoco>(@"{
                ""Score"": {
                    ""Name"": ""Alice"",
                    ""Score"": 1
                },
                ""NullScore"": {
                    ""Name"": ""Bob"",
                    ""Score"": 2
                }
            }", @"{
                ""Score"": {
                    ""Name"": ""Charlie"",
                    ""Score"": 3
                },
                ""NullScore"": null
            }");
        }
    }
}
