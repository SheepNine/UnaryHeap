using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Linq;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class EnumArrayPocoTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0, new EnumArrayPoco(Array.Empty<TrueBool>()).Nigredo.Count);
            var data = new TrueBool[] { TrueBool.False, TrueBool.True };
            var poco = new EnumArrayPoco(data);
            Assert.AreEqual(2, poco.Nigredo.Count);
            data[0] = TrueBool.FileNotFound; // Ensures poco made a copy
            Assert.AreEqual(TrueBool.False, poco.Nigredo[0]);
            Assert.AreEqual(TrueBool.True, poco.Nigredo[1]);
        }

        [Test]
        public void Checksum()
        {
            TestUtils.TestChecksum(
                new EnumArrayPoco(new[] { TrueBool.True, TrueBool.False }),
                "70a762c644adabb66f2b3ffe5633845a71b185d0c987ae207d1b63f2bc6dc82d");
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(() => new EnumArrayPoco(null));
        }

        [Test]
        public void Equality()
        {
            var data = new[] { TrueBool.True, TrueBool.False, TrueBool.FileNotFound };

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
            TestUtils.TestToString(new() { {
                new EnumArrayPoco(Array.Empty<TrueBool>()),
                @"{
                    Nigredo = []
                }"
            }, {
                new EnumArrayPoco(new TrueBool[] { TrueBool.FileNotFound }),
                @"{
                    Nigredo = [FileNotFound]
                }"
            }, {
                new EnumArrayPoco(new TrueBool[] { TrueBool.False, TrueBool.True }),
                @"{
                    Nigredo = [False, True]
                }"
            } });
        }

        [Test]
        public void RoundTrip()
        {
            var data = new[] { TrueBool.True, TrueBool.False, TrueBool.FileNotFound };
            TestUtils.TestRoundTrip(
                new EnumArrayPoco(data.Take(0)),
                new EnumArrayPoco(data.Take(1)),
                new EnumArrayPoco(data.Take(2))
            );
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<EnumArrayPoco>(@"{
                ""Nigredo"": []
            }", @"{
                ""Nigredo"": [""True""]
            }", @"{
                ""Nigredo"":[""False"",""FileNotFound""]
            }");
        }

        [Test]
        public void Builder()
        {
            var builder = new EnumArrayPoco(new[] {
                TrueBool.False, TrueBool.False, TrueBool.False
            }).ToBuilder();
            Assert.AreEqual(3, builder.NumNigredo);
            Assert.AreEqual(TrueBool.False, builder.GetNigredo(2));
            builder.SetNigredo(0, TrueBool.FileNotFound);
            builder.InsertNigredoAt(2, TrueBool.True);
            builder.RemoveNigredoAt(1);
            builder.AppendNigredo(TrueBool.FileNotFound);
            Assert.AreEqual(new[] {
                TrueBool.FileNotFound, TrueBool.True, TrueBool.False, TrueBool.FileNotFound
            }, builder.NigredoValues);
            var actual = builder.Build().Nigredo.ToArray();
            builder.ClearNigredo();
            Assert.AreEqual(0, builder.NumNigredo);
            Assert.AreEqual(new[] {
                TrueBool.FileNotFound, TrueBool.True, TrueBool.False, TrueBool.FileNotFound
            }, actual.ToArray());
        }
    }
}
