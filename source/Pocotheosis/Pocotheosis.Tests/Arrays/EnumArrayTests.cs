using Pocotheosis.Tests.Pocos;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    internal class EnumArrayTests: PocoTestFixture<EnumArray>
    {
        public EnumArrayTests()
        {
            AddSample(
                new EnumArray(Enumerable.Empty<TrueBool>()),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    Enums = []
                }",
                @"{
                    ""Enums"": []
                }");
            AddSample(
                new EnumArray(new TrueBool[] { TrueBool.FileNotFound }),
                "a1089b685c6f9b5e6d73c47486462d565ee583ee473f2f61661be2defd36f731",
                @"{
                    Enums = [FileNotFound]
                }",
                @"{
                    ""Enums"": [""FileNotFound""]
                }");
            AddSample(
                new EnumArray(new TrueBool[] { TrueBool.FileNotFound, TrueBool.False }),
                "2341b86a342b92256074d7f483442eff56c75bb344e92e8f825fbdef35dd478b",
                @"{
                    Enums = [FileNotFound, False]
                }",
                @"{
                    ""Enums"": [""FileNotFound"",""False""]
                }");
        }
    }
}
