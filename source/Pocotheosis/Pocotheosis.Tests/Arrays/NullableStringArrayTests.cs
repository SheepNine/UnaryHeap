using Pocotheosis.Tests.Pocos;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    internal class NullableStringArrayTests : PocoTestFixture<NullableStringArray>
    {
        public NullableStringArrayTests()
        {
            AddSample(
                new NullableStringArray(Enumerable.Empty<string>()),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    MaybeStrings = []
                }",
                @"{
                    ""MaybeStrings"": []
                }");
            AddSample(
                new NullableStringArray(new[] { "alpha" }),
                "8ccf9640fe1df853f835707b5322e91b7151ce6d7e19372639ad61a446d1f3dc",
                @"{
                    MaybeStrings = ['alpha']
                }",
                @"{
                    ""MaybeStrings"": [""alpha""]
                }");
            AddSample(
                new NullableStringArray(new string[] { null }),
                "b15348c8f462384c01e83b6d499c6faf3f96808f5aa07c6bab4b65b36b4445d4",
                @"{
                    MaybeStrings = [null]
                }",
                @"{
                    ""MaybeStrings"": [null]
                }");
            AddSample(
                new NullableStringArray(new[] { "omega", null }),
                "d63d130286bd6790badad7805069cd9bf583a009f1b39088e055a9c8e2b4b651",
                @"{
                    MaybeStrings = ['omega', null]
                }",
                @"{
                    ""MaybeStrings"": [""omega"",null]
                }");
        }
    }
}
