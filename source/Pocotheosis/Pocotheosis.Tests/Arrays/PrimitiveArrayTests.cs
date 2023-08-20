using Pocotheosis.Tests.Pocos;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    internal class PrimitiveArrayTests: PocoTestFixture<PrimitiveArray>
    {
        public PrimitiveArrayTests()
        {
            AddSample(
                new PrimitiveArray(Enumerable.Empty<byte>()),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    Primitives = []
                }",
                @"{
                    ""Primitives"": []
                }");
            AddSample(
                new PrimitiveArray(new byte[] { 1 }),
                "a1cb20470d89874f33383802c72d3c27a0668ebffd81934705ab0cfcbf1a1e3a",
                @"{
                    Primitives = [1]
                }",
                @"{
                    ""Primitives"": [1]
                }");
            AddSample(
                new PrimitiveArray(new byte[] { 205, 150, 79, 15 }),
                "c30af6a0711e8d68acbbcd3b1e9ecc11705bc2781beca3ac4d832dbb24b2a230",
                @"{
                    Primitives = [205, 150, 79, 15]
                }",
                @"{
                    ""Primitives"": [205,150,79,15]
                }");
        }
    }
}
