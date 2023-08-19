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
        public void ConstructorNullReference()
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
        public void Builder()
        {
            var sut = new ClassPoco(new ScoreTuple("Alice", 80));
            var endBuilder = sut.ToBuilder();
            Assert.AreEqual("Alice", endBuilder.Score.Name);
            Assert.AreEqual(80, endBuilder.Score.Score);
            endBuilder.Score.Name = "Colleen";
            endBuilder.Score.Score = 120;
            var end = endBuilder.Build();
            Assert.AreEqual("Colleen", end.Score.Name);
            Assert.AreEqual(120, end.Score.Score);
            endBuilder.WithScore(new ScoreTuple("Chris", 121));
            Assert.AreEqual("Chris", endBuilder.Build().Score.Name);
            Assert.AreEqual(121, endBuilder.Build().Score.Score);
        }

        [Test]
        public void BuilderNullValue()
        {
            Assert.Throws<System.ArgumentNullException>(() => new ClassPoco.Builder(null));
            Assert.Throws<System.ArgumentNullException>(() =>
            {
                new ClassPoco.Builder(new ScoreTuple("a", 1)).WithScore(null);
            });
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new ClassPoco(new ScoreTuple("Roger", 5)),
                "91d4fd239b243971c8a4e4d4413b98b21359aab5ac262498a1679053cc44b34b");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new ClassPoco(new ScoreTuple("Alice", 872)),
                @"{
                    Score = {
                        Name = 'Alice'
                        Score = 872
                    }
                }"
            }, {
                new ClassPoco(new ScoreTuple("Bob", 1)),
                @"{
                    Score = {
                        Name = 'Bob'
                        Score = 1
                    }
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new ClassPoco(new ScoreTuple("Alice", 872)),
                new ClassPoco(new ScoreTuple("Bob", 1))
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<ClassPoco>(@"{
                ""Score"": {
                    ""Name"": ""Alice"",
                    ""Score"": 872
                }
            }", @"{
                ""Score"": {
                    ""Name"": ""Bob"",
                    ""Score"": 1
                }
            }");
        }
    }
}
