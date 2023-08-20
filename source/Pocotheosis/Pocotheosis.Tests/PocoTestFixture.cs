using Newtonsoft.Json;
using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Pocotheosis.Tests
{
    [TestFixture]
    internal abstract class PocoTestFixture<TPoco> where TPoco : Poco
    {
        private List<TPoco> Pocos = new List<TPoco>();
        private List<string> Checksums = new List<string>();
        private List<string> StringFormats = new List<string>();
        private List<string> JsonFormats = new List<string>();

        protected void AddSample(TPoco poco, string checksum, string stringFormat,
            string jsonFormat)
        {
            Pocos.Add(poco);
            Checksums.Add(checksum);
            StringFormats.Add(stringFormat
                    .Replace("                ", string.Empty)
                    .Replace("    ", "\t"));
            JsonFormats.Add(jsonFormat
                    .Replace(": ", ":")
                    .Replace("    ", string.Empty)
                    .Replace(Environment.NewLine, string.Empty));
        }

        protected static bool EquatableHelper_AreEqual(TPoco a, TPoco b)
        {
            return (bool)typeof(EquatableHelper).GetMethod("AreEqual",
                BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(TPoco), typeof(TPoco) })
                    .Invoke(null, new object[] { a, b });
        }

        protected static void PocoJson_Serialize(TPoco poco, JsonTextWriter writer)
        {
            typeof(PocoJson).GetMethod("Serialize",
                BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(TPoco), typeof(JsonTextWriter) })
                    .Invoke(null, new object[] { poco, writer });
        }

        protected static string WriteToJson(TPoco poco)
        {
            using (var textWriter = new StringWriter())
            {
                using (var jsonWriter = new JsonTextWriter(textWriter))
                    PocoJson_Serialize(poco, jsonWriter);

                return textWriter.ToString();
            }
        }

        protected static TPoco PocoJson_DeserializeTPoco(JsonTextReader reader, bool isNullable)
        {
            return (TPoco)typeof(PocoJson).GetMethod(
                "Deserialize" + typeof(TPoco).Name,
                BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(JsonTextReader), typeof(bool) })
                    .Invoke(null, new object[] { reader, isNullable });
        }

        protected static TPoco ReadFromJson(string jsonText, bool isNullable)
        {
            using (var reader = new JsonTextReader(new StringReader(jsonText)))
                return PocoJson_DeserializeTPoco(reader, isNullable);
        }

        [Test]
        public void HashCode()
        {
            foreach (var i in Enumerable.Range(0, Pocos.Count))
                Assert.DoesNotThrow(() => Pocos[i].GetHashCode());
        }

        [Test]
        public void Equality()
        {
            foreach (var i in Enumerable.Range(0, Pocos.Count))
            {
                var pocoI = Pocos[i];
                Assert.False(pocoI.Equals(null));
                Assert.False(EquatableHelper_AreEqual(pocoI, null));
                Assert.False(EquatableHelper_AreEqual(null, pocoI));

                foreach (var j in Enumerable.Range(0, Pocos.Count))
                {
                    var pocoJ = Pocos[j];
                    var expected = i == j;
                    Assert.AreEqual(expected, pocoI.Equals(pocoJ));
                    Assert.AreEqual(expected, pocoJ.Equals(pocoI));
                    Assert.AreEqual(expected, EquatableHelper_AreEqual(pocoI, pocoJ));
                    Assert.AreEqual(expected, EquatableHelper_AreEqual(pocoJ, pocoI));
                }
            }
        }

        [Test]
        public void Checksum()
        {
            foreach (var i in Enumerable.Range(0, Pocos.Count))
                Assert.AreEqual(Checksums[i], Pocos[i].Checksum);
        }

        [Test]
        public void StringFormat()
        {
            foreach (var i in Enumerable.Range(0, Pocos.Count))
                Assert.AreEqual(StringFormats[i], Pocos[i].ToString());
        }


        [Test]
        public void JsonFormat()
        {
            // Can read null object (if nullable)
            Assert.IsNull(ReadFromJson("null", true));

            // Can write a null value
            Assert.AreEqual("null", WriteToJson(null));

            // Cannot read null object (if not nullable)   
            Assert.Throws<InvalidDataException>(() => {
                try
                {
                    ReadFromJson("null", false);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            });

            // Read of unexpected property name is an error
            Assert.Throws<InvalidDataException>(() => {
                try
                {
                    ReadFromJson(@"{""not_a_value"": null}", false);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            });

            // Run through each individual test case
            foreach (var i in Enumerable.Range(0, Pocos.Count))
            {
                Assert.AreEqual(JsonFormats[i], WriteToJson(Pocos[i]));
                Assert.AreEqual(Pocos[i], ReadFromJson(JsonFormats[i], false));
            }
        }

        private static void SerializationHelpers_Serialize(TPoco value, Stream output)
        {
            typeof(SerializationHelpers).GetMethod(
                    "Serialize",
                    BindingFlags.Static | BindingFlags.Public,
                    new[] { typeof(TPoco), typeof(Stream) })
                .Invoke(null, new object[] { value, output });
        }

        private static void SerializationHelpers_SerializeWithId(TPoco value, Stream output)
        {
            typeof(SerializationHelpers).GetMethod(
                    "SerializeWithId",
                    BindingFlags.Static | BindingFlags.Public,
                    new[] { typeof(TPoco), typeof(Stream) })
                .Invoke(null, new object[] { value, output });
        }

        private static TPoco TPoco_Deserialize(Stream input)
        {
            return (TPoco)typeof(TPoco).GetMethod(
                    "Deserialize",
                    BindingFlags.Static | BindingFlags.Public,
                    new[] { typeof(Stream) })
                .Invoke(null, new object[] { input });
        }

        [Test]
        public void Serialization()
        {
            foreach (var i in Enumerable.Range(0, Pocos.Count))
            {
                using (var stream = new MemoryStream())
                {
                    using (var reader = new PocoReader(stream))
                    using (var writer = new PocoWriter(stream))
                    {
                        writer.Send(Pocos[i]).Flush();
                        stream.Seek(0, SeekOrigin.Begin);
                        var actual = reader.Receive();
                        Assert.AreEqual(Pocos[i], actual);
                    }
                }

                using (var stream = new MemoryStream())
                {
                    SerializationHelpers_Serialize(Pocos[i], stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    var actual = TPoco_Deserialize(stream);
                    Assert.AreEqual(Pocos[i], actual);
                }

                using (var stream = new MemoryStream())
                {
                    SerializationHelpers_SerializeWithId(Pocos[i], stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    var actual = Poco.DeserializeWithId<TPoco>(stream);
                    Assert.AreEqual(Pocos[i], actual);
                }
            }
        }
    }
}
