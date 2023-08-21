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
                "35be322d094f9d154a8aba4733b8497f180353bd7ae7b0a15f90b586b549f28b",
                @"{
                    Primitives = []
                }",
                @"{
                    ""Primitives"": []
                }");
            AddSample(
                new PrimitiveArray(new byte[] { 1 }),
                "86acb7e5dfa7228759e774443eedf64fbbace2ab942dde45340b356937ef4c0e",
                @"{
                    Primitives = [1]
                }",
                @"{
                    ""Primitives"": [1]
                }");
            AddSample(
                new PrimitiveArray(new byte[] { 205, 150, 79, 15 }),
                "3343ec2e0152367b5e0500fb683efe5884b026221e5eeeb85ef8d6e3fa11565e",
                @"{
                    Primitives = [205, 150, 79, 15]
                }",
                @"{
                    ""Primitives"": [205,150,79,15]
                }");

            AddInvalidConstructions(
                () => { var a = new PrimitiveArray(null); }
            );
        }
    }
}
