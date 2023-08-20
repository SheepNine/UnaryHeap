using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    [TestFixture]
    public class EnumArrayTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0, new EnumArray(Array.Empty<TrueBool>()).Elements.Count);
            var data = new TrueBool[] { TrueBool.False, TrueBool.True };
            var poco = new EnumArray(data);
            Assert.AreEqual(2, poco.Elements.Count);
            data[0] = TrueBool.FileNotFound; // Ensures poco made a copy
            Assert.AreEqual(TrueBool.False, poco.Elements[0]);
            Assert.AreEqual(TrueBool.True, poco.Elements[1]);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(() => new EnumArray(null));
        }

        [Test]
        public void Equality()
        {
            var data = new[] { TrueBool.True, TrueBool.False, TrueBool.FileNotFound };

            Assert.AreEqual(new EnumArray(data.Take(2)),
                new EnumArray(data.Take(2)));
            Assert.AreNotEqual(new EnumArray(data.Take(1)),
                new EnumArray(data.Take(2)));
            Assert.AreNotEqual(new EnumArray(data.Take(3)),
                new EnumArray(data.Take(2)));
            Assert.AreNotEqual(new EnumArray(data),
                new EnumArray(data.Reverse()));
        }

        [Test]
        public void Builder()
        {
            var builder = new EnumArray(new[] {
                TrueBool.False, TrueBool.False, TrueBool.False
            }).ToBuilder();
            Assert.AreEqual(3, builder.NumElements);
            Assert.AreEqual(TrueBool.False, builder.GetElement(2));
            builder.SetElement(0, TrueBool.FileNotFound);
            builder.InsertElementAt(2, TrueBool.True);
            builder.RemoveElementAt(1);
            builder.AppendElement(TrueBool.FileNotFound);
            Assert.AreEqual(new[] {
                TrueBool.FileNotFound, TrueBool.True, TrueBool.False, TrueBool.FileNotFound
            }, builder.ElementValues);
            var actual = builder.Build().Elements.ToArray();
            builder.ClearElements();
            Assert.AreEqual(0, builder.NumElements);
            Assert.AreEqual(new[] {
                TrueBool.FileNotFound, TrueBool.True, TrueBool.False, TrueBool.FileNotFound
            }, actual.ToArray());
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new EnumArray(new[] { TrueBool.True, TrueBool.False }),
                "70a762c644adabb66f2b3ffe5633845a71b185d0c987ae207d1b63f2bc6dc82d");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new EnumArray(Array.Empty<TrueBool>()),
                @"{
                    Elements = []
                }"
            }, {
                new EnumArray(new TrueBool[] { TrueBool.FileNotFound }),
                @"{
                    Elements = [FileNotFound]
                }"
            }, {
                new EnumArray(new TrueBool[] { TrueBool.False, TrueBool.True }),
                @"{
                    Elements = [False, True]
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            var data = new[] { TrueBool.True, TrueBool.False, TrueBool.FileNotFound };
            PocoTest.Serialization(
                new EnumArray(data.Take(0)),
                new EnumArray(data.Take(1)),
                new EnumArray(data.Take(2))
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<EnumArray>(@"{
                ""Elements"": []
            }", @"{
                ""Elements"": [""True""]
            }", @"{
                ""Elements"":[""False"",""FileNotFound""]
            }");
        }
    }
}
