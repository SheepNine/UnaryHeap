using NUnit.Framework;
using GeneratedTestPocos;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    internal class PolymorphicArrayTests : PocoTestFixture<PolymorphicArray>
    {
        static PolymorphicArray.Builder EmptyBuilder
        {
            get { return new PolymorphicArray.Builder(Enumerable.Empty<IPoco>()); }
        }

        public PolymorphicArrayTests()
        {
            AddSample(
                new PolymorphicArray(Enumerable.Empty<PrimitiveValue>()),
                "1051932bfab6df8df143726eecc92885ebcf1008b0685f8e85fb0a22797244a0",
                @"{
                    Rainbows = []
                }",
                @"{
                    ""Rainbows"": []
                }");
            AddSample(
                new PolymorphicArray(new[] { P(88) }),
                "62cf2b00a59e7d1391047e117140569bf857a4989cbfe133f5f532e5cd4248a8",
                @"{
                    Rainbows = [{
                        Primitive = 88
                    }]
                }",
                @"{
                    ""Rainbows"": [{
                        ""type"": ""PrimitiveValue"",
                        ""data"": {
                            ""Primitive"": 88
                        }
                    }]
                }");
            AddSample(
                new PolymorphicArray(new IPoco[] { P(9), new EnumValue(Fls) }),
                "b960aa71362681f0df7649fdb4372066790ba1c8bc17be3c1ffdfe92884140a2",
                @"{
                    Rainbows = [{
                        Primitive = 9
                    }, {
                        Enum = False
                    }]
                }",
                @"{
                    ""Rainbows"": [{
                        ""type"": ""PrimitiveValue"",
                        ""data"": {
                            ""Primitive"": 9
                        }
                    },{
                        ""type"": ""EnumValue"",
                        ""data"": {
                            ""Enum"": ""False""
                        }
                    }]
                }");

            AddInvalidConstructions(
                () => { var a = new PolymorphicArray(null); },
                () => { var a = new PolymorphicArray(new PrimitiveValue[] { null }); },
                () => { var a = new PolymorphicArray.Builder(null); },
                () => { var a = new PolymorphicArray.Builder(new PrimitiveValue[] { null }); },
                () => { EmptyBuilder.WithRainbows(null); },
                () => { EmptyBuilder.WithRainbows(new IPoco[] { null }); },
                () => { EmptyBuilder.AppendRainbow(null); },
                () => { EmptyBuilder.InsertRainbowAt(0, null); },
                () => { EmptyBuilder.SetRainbow(0, null); }
            );
        }

        [Test]
        public override void Builder()
        {
            var A = P(1);
            var B = P(3);
            var C = P(5);

            var sut = new PolymorphicArray(new[] { A, B, C }).ToBuilder();
            sut.SetRainbow(2, A);
            sut.RemoveRainbowAt(1);
            Assert.AreEqual(2, sut.NumRainbows);
            Assert.AreEqual(A, sut.GetRainbow(1));
            Assert.AreEqual(new[] { A, A }, sut.RainbowValues);

            sut.ClearRainbows();
            sut.AppendRainbow(B);
            sut.InsertRainbowAt(0, C);
            sut.InsertRainbowAt(2, A);
            Assert.AreEqual(
                EmptyBuilder.WithRainbows(new[] { C, B, A }).Build(),
                sut.Build());
        }
    }
}
