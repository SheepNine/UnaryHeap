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
                "23d7f42b1cdc1f0d492ebd756ed0fe8003995dda554d99418d47a81813650207",
                @"{
                    Enums = []
                }",
                @"{
                    ""Enums"": []
                }");
            AddSample(
                new EnumArray(new TrueBool[] { TrueBool.FileNotFound }),
                "4868753daf74d1496daefe698cd446b270a8989ec8ee182948035c72986ca770",
                @"{
                    Enums = [FileNotFound]
                }",
                @"{
                    ""Enums"": [""FileNotFound""]
                }");
            AddSample(
                new EnumArray(new TrueBool[] { TrueBool.FileNotFound, TrueBool.False }),
                "3ed750f1c4939b6200950cb27d8bae9627c10b848f2fa40d9e6872d2ae948638",
                @"{
                    Enums = [FileNotFound, False]
                }",
                @"{
                    ""Enums"": [""FileNotFound"",""False""]
                }");
        }
    }
}
