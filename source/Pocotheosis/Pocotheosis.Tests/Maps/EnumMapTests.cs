using NUnit.Framework;
using GeneratedTestPocos;
using Dataset = System.Collections.Generic.Dictionary<byte, GeneratedTestPocos.TrueBool>;
using KV = System.Collections.Generic.KeyValuePair<byte, GeneratedTestPocos.TrueBool>;

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
                new EnumMap(new Dataset() { { 5, Tru } }),
                "83df55090bcaa83e2ee0e8dba3e20d341edc9448ddfa7a4953b0a1cbfd66a5ad",
                @"{
                    Enums = (
                        5 -> True
                    )
                }",
                @"{
                    ""Enums"": [{
                        ""k"": 5,
                        ""v"": ""True""
                    }]
                }");
            AddSample(
                new EnumMap(new Dataset() {
                    { 5, Tru },
                    { 77, FNF }
                }),
                "33608851f15836fd3161258093aa66b8f2e827504f3a92556ab6ebd002cffa33",
                @"{
                    Enums = (
                        5 -> True,
                        77 -> FileNotFound
                    )
                }",
                @"{
                    ""Enums"": [{
                        ""k"": 5,
                        ""v"": ""True""
                    },{
                        ""k"": 77,
                        ""v"": ""FileNotFound""
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
            var Va = Fls;
            var Vb = Tru;
            var Vc = FNF;
            var Vd = Fls;

            var sut = new EnumMap(new Dataset() { { Ka, Va }, { Kb, Vb } }).ToBuilder();
            sut.SetEnum(Ka, Vc);
            sut.RemoveEnum(Kb);
            Assert.True(sut.ContainsEnumKey(Ka));
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
