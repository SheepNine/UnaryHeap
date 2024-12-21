using NUnit.Framework;
using GeneratedTestPocos;

namespace Pocotheosis.Tests.Values
{
    public class NullableStringValueTests : PocoTestFixture<NullableStringValue>
    {
        public NullableStringValueTests()
        {
            AddSample(
                new NullableStringValue(string.Empty),
                "5c62e2f48f43961a204fd130c976b94999cda7d74de4ace9e5310a634fc7381e",
                @"{
                    MaybeString = ''
                }",
                @"{
                    ""MaybeString"": """"
                }");
            AddSample(
                new NullableStringValue("a regular string"),
                "c57e6801b38fc700bc67b42e889293a91183781663274aa22f380892c854a407",
                @"{
                    MaybeString = 'a regular string'
                }",
                @"{
                    ""MaybeString"": ""a regular string""
                }");
            AddSample(
                new NullableStringValue(null),
                "06bb2a58eee03af90cdb05f53676be0e24cc6f16864276fac603e960c9b97fc5",
                @"{
                    MaybeString = null
                }",
                @"{
                    ""MaybeString"": null
                }");

            NoInvalidConstructions();
        }

        [Test]
        public override void Builder()
        {
            var sut = new NullableStringValue("alpha").ToBuilder();
            Assert.AreEqual("alpha", sut.MaybeString);
            Assert.AreEqual(
                sut.WithMaybeString("beta").Build(),
                new NullableStringValue.Builder("beta").Build());
            sut.MaybeString = null;
            Assert.IsNull(sut.MaybeString);
            Assert.AreEqual(
                sut.WithMaybeString(null).Build(),
                new NullableStringValue.Builder(null).Build());
        }
    }
}
