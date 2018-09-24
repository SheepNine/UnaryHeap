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
            Assert.AreEqual("{\r\n\tNigredo = [ ]\r\n}",
                new EnumArrayPoco(new TestEnum[] { }).ToString());
            Assert.AreEqual("{\r\n\tNigredo = [ False ]\r\n}",
                new EnumArrayPoco(new TestEnum[] { TestEnum.False }).ToString());
            Assert.AreEqual("{\r\n\tNigredo = [ False, True ]\r\n}",
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

        [Test]
        public void Builder()
        {
            var builder = new EnumArrayPoco(new[] {
                TestEnum.False, TestEnum.False, TestEnum.False
            }).ToBuilder();
            Assert.AreEqual(3, builder.NumNigredo);
            Assert.AreEqual(TestEnum.False, builder.GetNigredo(2));
            builder.SetNigredo(0, TestEnum.FileNotFound);
            builder.InsertNigredoAt(2, TestEnum.True);
            builder.RemoveNigredoAt(1);
            builder.AppendNigredo(TestEnum.FileNotFound);
            Assert.AreEqual(new[] {
                TestEnum.FileNotFound, TestEnum.True, TestEnum.False, TestEnum.FileNotFound
            }, builder.NigredoValues);
            var actual = builder.Build().Nigredo.ToArray();
            builder.ClearNigredo();
            Assert.AreEqual(0, builder.NumNigredo);
            Assert.AreEqual(new[] {
                TestEnum.FileNotFound, TestEnum.True, TestEnum.False, TestEnum.FileNotFound
            }, actual.ToArray());
        }
    }
}
