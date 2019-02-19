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
    }
}
