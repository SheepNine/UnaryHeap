using Newtonsoft.Json;
using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Pocotheosis.Tests
{
    static class PocoTest
    {
        public static void Serialization(params Poco[] pocos)
        {
            foreach (var poco in pocos)
            {
                var stream = new MemoryStream();
                new PocoWriter(stream).Send(poco).Flush();

                stream.Seek(0, SeekOrigin.Begin);

                var roundTrip = new PocoReader(stream).Receive();
                Assert.AreEqual(poco, roundTrip);
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
