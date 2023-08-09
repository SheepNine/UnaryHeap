using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class ClassDictionaryPocoTests
    {
        [Test]
        [Ignore("TODO")]
        public void Constructor()
        {
        }

        [Test]
        [Ignore("TODO")]
        public void Constructor_NullReference()
        {
        }

        [Test]
        [Ignore("TODO")]
        public void Equality()
        {
        }

        [Test]
        [Ignore("TODO")]
        public void StringFormat()
        {
        }

        [Test]
        [Ignore("TODO")]
        public void RoundTrip()
        {

        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<ClassDictionaryPoco>(
                @"{""Geese"":[]}");
            TestUtils.TestJsonRoundTrip<ClassDictionaryPoco>(
                @"{""Geese"":[{""k"":3,""v"":{""Value"":true}}]}");
            TestUtils.TestJsonRoundTrip<ClassDictionaryPoco>(
                @"{""Geese"":[{""k"":3,""v"":{""Value"":true}},"
                    + @"{""k"":5,""v"":{""Value"":false}}]}");
        }
    }
}
