using Newtonsoft.Json;
using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System.IO;

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

        public static void TestJsonRoundTrip(string json)
        {
            Poco poco;
            using (var reader = new JsonTextReader(new StringReader(json)))
                poco = EmptyPoco.Deserialize(reader);

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
