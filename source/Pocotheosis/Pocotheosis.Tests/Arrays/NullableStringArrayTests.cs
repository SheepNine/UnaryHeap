using NUnit.Framework;
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
                "189fe9cc9594aa36c1120fce9fa3de63dec3bc9de0c7ccda6b259d5e6bf21dff",
                @"{
                    MaybeStrings = []
                }",
                @"{
                    ""MaybeStrings"": []
                }");
            AddSample(
                new NullableStringArray(new[] { "alpha" }),
                "71042ac423a3b9b6485083f4525dac71531e5c9680377a69f1929be317d62d06",
                @"{
                    MaybeStrings = ['alpha']
                }",
                @"{
                    ""MaybeStrings"": [""alpha""]
                }");
            AddSample(
                new NullableStringArray(new string[] { null }),
                "251f0892771c2835be99c3424cdd455a0424984ad206491871df6eb996ea5ea3",
                @"{
                    MaybeStrings = [null]
                }",
                @"{
                    ""MaybeStrings"": [null]
                }");
            AddSample(
                new NullableStringArray(new[] { "omega", null }),
                "28229fdef61e27152493215c72223b5c71ca633bdd0fe75befd1eb2f36d89e16",
                @"{
                    MaybeStrings = ['omega', null]
                }",
                @"{
                    ""MaybeStrings"": [""omega"",null]
                }");

            AddInvalidConstructions(
                () => { var a = new NullableStringArray(null); },
                () => { var a = new NullableStringArray.Builder(null); }
            );
        }

        [Test]
        public override void Builder()
        {
            var A = null as string;
            var B = "alpha";
            var C = "beta";

            var sut = new NullableStringArray(new[] { A, B, C }).ToBuilder();
            sut.SetMaybeString(2, A);
            sut.RemoveMaybeStringAt(1);
            Assert.AreEqual(2, sut.NumMaybeStrings);
            Assert.AreEqual(A, sut.GetMaybeString(1));
            Assert.AreEqual(new[] { A, A }, sut.MaybeStringValues);

            sut.ClearMaybeStrings();
            sut.AppendMaybeString(B);
            sut.InsertMaybeStringAt(0, C);
            sut.InsertMaybeStringAt(2, A);
            Assert.AreEqual(
                new NullableStringArray.Builder(new[] { C, B, A }).Build(),
                sut.Build());
        }
    }
}
