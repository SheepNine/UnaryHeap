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
    static class PocoTest
    {
        public static void Equality<T>(params T[] pocos)
        {
            var staticEquality = typeof(EquatableHelper).GetMethod(
                "AreEqual",
                BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(T), typeof(T) });


            foreach (var i in Enumerable.Range(0, pocos.Length))
            {
                var pocoI = pocos[i];
                Assert.False(pocoI.Equals(null));
                Assert.False((bool)staticEquality.Invoke(null, new object[] { pocoI, null }));
                Assert.False((bool)staticEquality.Invoke(null, new object[] { null, pocoI }));

                foreach (var j in Enumerable.Range(0, pocos.Length))
                {
                    var pocoJ = pocos[j];
                    var expected = i == j;
                    Assert.AreEqual(expected, pocoI.Equals(pocoJ));
                    Assert.AreEqual(expected, pocoJ.Equals(pocoI));
                    Assert.AreEqual(expected,
                        (bool)staticEquality.Invoke(null, new object[] { pocoI, pocoJ }));
                    Assert.AreEqual(expected,
                        (bool)staticEquality.Invoke(null, new object[] { pocoJ, pocoI }));
                }
            }
        }

        public static void Serialization(params Poco[] pocos)
        {
            foreach (var poco in pocos)
            {
                Assert.DoesNotThrow(() => poco.GetHashCode());

                var stream = new MemoryStream();
                var writer = new PocoWriter(stream);
                writer.Send(poco).Flush();

                stream.Seek(0, SeekOrigin.Begin);

                var reader = new PocoReader(stream);
                var roundTrip = reader.Receive();
                Assert.AreEqual(poco, roundTrip);

                reader.Dispose();
                writer.Dispose();
                stream.Dispose();
            }
        }

        public static void JsonSerialization<T>(params string[] jsons)
        {
            var deserializer = typeof(PocoJson).GetMethod(
                "Deserialize" + typeof(T).Name,
                BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(JsonTextReader), typeof(bool) });

            var serializer = typeof(PocoJson).GetMethod(
                "Serialize",
                BindingFlags.Static | BindingFlags.Public,
                new[] { typeof(T), typeof(JsonTextWriter) });

            // Boost coverage with some standard tests that work for every POCO class

            // Read of null returns null (if allowed)
            using (var reader = new JsonTextReader(new StringReader("null")))
                Assert.IsNull(deserializer.Invoke(null, new object[] { reader, true }));

            // Read of null throws error (if not allowed)
            using (var reader = new JsonTextReader(new StringReader("null")))
                Assert.Throws<InvalidDataException>(() => {
                    try
                    {
                        deserializer.Invoke(null, new object[] { reader, false });
                    } catch (TargetInvocationException ex)
                    {
                        throw ex.InnerException;
                    }
                });

            // Read of unexpected property name is an error
            using (var reader = new JsonTextReader(new StringReader(@"{""not_a_value"": null}")))
                Assert.Throws<InvalidDataException>(() => {
                    try
                    {
                        deserializer.Invoke(null, new object[] { reader, false });
                    }
                    catch (TargetInvocationException ex)
                    {
                        throw ex.InnerException;
                    }
                });

            // Write of null writes null
            using (var writeStream = new StringWriter())
            {
                using (var writer = new JsonTextWriter(writeStream))
                    serializer.Invoke(null, new object[] { null, writer });

                Assert.AreEqual("null", writeStream.ToString());
            }

            foreach (var json in jsons)
            {
                Poco poco;
                using (var reader = new JsonTextReader(new StringReader(json)))
                    poco = (Poco)deserializer.Invoke(null, new object[] { reader, false });

                var stream = new MemoryStream();
                using (var stringWriter = new StringWriter())
                {
                    using (var jsonWriter = new JsonTextWriter(stringWriter))
                    {
                        serializer.Invoke(null, new object[] { poco, jsonWriter });
                        jsonWriter.Flush();
                    }

                    Assert.AreEqual(
                        json.Replace(": ", ":").Replace("    ", string.Empty)
                            .Replace(Environment.NewLine, string.Empty),
                        stringWriter.ToString());
                }
            }
        }

        public static void StringFormat(Dictionary<Poco, string> testCases)
        {
            foreach (var testCase in testCases)
                Assert.AreEqual(
                    testCase.Value
                        .Replace("                ", string.Empty)
                        .Replace("    ", "\t"),
                    testCase.Key.ToString());
        }

        public static void Checksum(Poco poco, string expected)
        {
            Assert.AreEqual(expected, poco.Checksum);
        }
    }
}
