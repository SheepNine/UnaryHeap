using Pocotheosis.Tests.Pocos;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    internal class StringArrayTests: PocoTestFixture<StringArray>
    {
        public StringArrayTests()
        {
            AddSample(
                new StringArray(Enumerable.Empty<string>()),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    Strs = []
                }",
                @"{
                    ""Strs"": []
                }");
            AddSample(
                new StringArray(new[] { "alpha" }),
                "8ccf9640fe1df853f835707b5322e91b7151ce6d7e19372639ad61a446d1f3dc",
                @"{
                    Strs = ['alpha']
                }",
                @"{
                    ""Strs"": [""alpha""]
                }");
            AddSample(
                new StringArray(new[] { "omega", "alpha" }),
                "7206c47ce08968f97840b0a193fc247fe3d628309e9ff789ea2a19e74d4ec2f7",
                @"{
                    Strs = ['omega', 'alpha']
                }",
                @"{
                    ""Strs"": [""omega"",""alpha""]
                }");
        }
    }
}
