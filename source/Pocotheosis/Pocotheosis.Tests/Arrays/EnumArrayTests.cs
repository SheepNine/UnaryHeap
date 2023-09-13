using NUnit.Framework;
using GeneratedTestPocos;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    internal class EnumArrayTests : PocoTestFixture<EnumArray>
    {
        static EnumArray.Builder EmptyBuilder
        {
            get { return new EnumArray.Builder(Enumerable.Empty<TrueBool>()); }
        }

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
                new EnumArray(new TrueBool[] { FNF }),
                "4868753daf74d1496daefe698cd446b270a8989ec8ee182948035c72986ca770",
                @"{
                    Enums = [FileNotFound]
                }",
                @"{
                    ""Enums"": [""FileNotFound""]
                }");
            AddSample(
                new EnumArray(new TrueBool[] { FNF, Fls}),
                "3ed750f1c4939b6200950cb27d8bae9627c10b848f2fa40d9e6872d2ae948638",
                @"{
                    Enums = [FileNotFound, False]
                }",
                @"{
                    ""Enums"": [""FileNotFound"",""False""]
                }");

            AddInvalidConstructions(
                () => { var a = new EnumArray(null); },
                () => { var a = new EnumArray.Builder(null); },
                () => { EmptyBuilder.WithEnums(null); }
            );
        }

        [Test]
        public override void Builder()
        {
            var A = Tru;
            var B = Fls;
            var C = FNF;

            var sut = new EnumArray(new[] { A, B, C }).ToBuilder();
            sut.SetEnum(2, A);
            sut.RemoveEnumAt(1);
            Assert.AreEqual(2, sut.NumEnums);
            Assert.AreEqual(A, sut.GetEnum(1));
            Assert.AreEqual(new[] { A, A }, sut.EnumValues);

            sut.ClearEnums();
            sut.AppendEnum(B);
            sut.InsertEnumAt(0, C);
            sut.InsertEnumAt(2, A);
            Assert.AreEqual(
                EmptyBuilder.WithEnums(new[] { C, B, A }).Build(),
                sut.Build());
        }
    }
}
