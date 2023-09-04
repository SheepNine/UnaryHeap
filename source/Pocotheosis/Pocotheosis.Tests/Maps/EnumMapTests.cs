using NUnit.Framework;
using GeneratedTestPocos;
using Dataset = System.Collections.Generic.Dictionary<byte, GeneratedTestPocos.LatinLetters>;
using KV = System.Collections.Generic.KeyValuePair<byte, GeneratedTestPocos.LatinLetters>;

namespace Pocotheosis.Tests.Maps
{
    internal class EnumMapTests : PocoTestFixture<EnumMap>
    {
        public EnumMapTests()
        {
            AddSample(
                new EnumMap(new Dataset()),
                "9d440eeeca9570b4064dca02133b3c4960bdc7615e04d024d61b3a897c37d5df",
                @"{
                    Enums = ()
                }",
                @"{
                    ""Enums"": []
                }");
            AddSample(
                new EnumMap(new Dataset() { { 5, Alpha } }),
                "21e4632daacf5200474f6434f0a39d0c93e85cb91ff99786646610d826f1cea0",
                @"{
                    Enums = (
                        5 -> Alpha
                    )
                }",
                @"{
                    ""Enums"": [{
                        ""k"": 5,
                        ""v"": ""Alpha""
                    }]
                }");
            AddSample(
                new EnumMap(new Dataset() {
                    { 5, Alpha },
                    { 77, AlphaBeta }
                }),
                "0d2e23800569e17fd8d57e696c251b95f3a7d89475d48c7ed2204ff7df079ea4",
                @"{
                    Enums = (
                        5 -> Alpha,
                        77 -> AlphaBeta
                    )
                }",
                @"{
                    ""Enums"": [{
                        ""k"": 5,
                        ""v"": ""Alpha""
                    },{
                        ""k"": 77,
                        ""v"": ""AlphaBeta""
                    }]
                }");

            AddInvalidConstructions(
                () => { var a = new EnumMap(null); },
                () => { var a = new EnumMap.Builder(null); }
            );
        }

        [Test]
        public override void Builder()
        {
            var Ka = (byte)1;
            var Kb = (byte)8;
            var Kc = (byte)15;
            var Kd = (byte)22;
            var Va = Beta;
            var Vb = Alpha;
            var Vc = AlphaBeta;
            var Vd = Beta;

            var sut = new EnumMap(new Dataset() { { Ka, Va }, { Kb, Vb } }).ToBuilder();
            sut.SetEnum(Ka, Vc);
            sut.RemoveEnum(Kb);
            Assert.IsTrue(sut.ContainsEnumKey(Ka));
            Assert.False(sut.ContainsEnumKey(Kb));
            Assert.AreEqual(1, sut.CountEnums);
            Assert.AreEqual(Vc, sut.GetEnum(Ka));
            Assert.AreEqual(new[] { Ka }, sut.EnumKeys);
            Assert.AreEqual(new[] { new KV(Ka, Vc) }, sut.EnumValues);

            sut.ClearEnums();
            Assert.AreEqual(0, sut.CountEnums);

            sut.SetEnum(Kc, Vc);
            sut.SetEnum(Kd, Vd);
            Assert.AreEqual(
                new EnumMap.Builder(new Dataset() { { Kc, Vc }, { Kd, Vd } }).Build(),
                sut.Build());
        }
    }
}
