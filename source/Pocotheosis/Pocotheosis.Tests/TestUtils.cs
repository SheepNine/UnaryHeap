using Newtonsoft.Json;
using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System.IO;
using System.Reflection;

namespace Pocotheosis.Tests
{
    static class TestUtils
    {
        public static void TestRoundTrip(Poco poco)
        {
            var stream = new MemoryStream();
            new PocoWriter(stream).Send(poco).Flush();

            stream.Seek(0, SeekOrigin.Begin);

            var roundTrip = new PocoReader(stream).Receive();
            Assert.AreEqual(poco, roundTrip);
        }

        public static void TestJsonRoundTrip<T>(string json)
        {
            var deserializer = typeof(T).GetMethod("Deserialize", BindingFlags.Static | BindingFlags.Public, new[] { typeof(JsonTextReader) });

            Poco poco;
            using (var reader = new JsonTextReader(new StringReader(json)))
                poco = (Poco)deserializer.Invoke(null, new[] { reader });

            var stream = new MemoryStream();
            using (var stringWriter = new StringWriter())
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    poco.Serialize(jsonWriter);
                    jsonWriter.Flush();
                }

                Assert.AreEqual(json, stringWriter.ToString());
            }
        }
    }
}
