using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class EnumKeyDictionaryPocoTests
    {
        [Test]
        public void JsonSerialization()
        {
            TestUtils.TestJsonRoundTrip<EnumKeyDictionaryPoco>(
                @"{""Entries"":{""True"":6,""False"":1,""FileNotFound"":4}}");
        }
    }
}
