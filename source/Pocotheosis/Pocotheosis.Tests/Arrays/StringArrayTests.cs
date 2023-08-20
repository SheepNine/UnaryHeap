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
                "7f962dc8eb9ff177eb0a6f02349ec2883eaa1eac4ef3c55c346d2e3a508b0c4d",
                @"{
                    Strs = []
                }",
                @"{
                    ""Strs"": []
                }");
            AddSample(
                new StringArray(new[] { "alpha" }),
                "519565dcf6353cad06a6895d06f32f8f4155685d61434faad5f199e68c410d72",
                @"{
                    Strs = ['alpha']
                }",
                @"{
                    ""Strs"": [""alpha""]
                }");
            AddSample(
                new StringArray(new[] { "omega", "alpha" }),
                "0887efcd121b30dd297f5c376074727159810a1a8459a51aa3e869836e032051",
                @"{
                    Strs = ['omega', 'alpha']
                }",
                @"{
                    ""Strs"": [""omega"",""alpha""]
                }");
        }
    }
}
