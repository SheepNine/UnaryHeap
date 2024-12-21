using NUnit.Framework;
using GeneratedTestPocos;
using Dataset = System.Collections.Generic.Dictionary<int, string>;
using KV = System.Collections.Generic.KeyValuePair<int, string>;

namespace Pocotheosis.Tests.Maps
{
    public class NullableStringMapTests : PocoTestFixture<NullableStringMap>
    {
        public NullableStringMapTests()
        {
            AddSample(
                new NullableStringMap(new Dataset()),
                "7ba37aa08ed85296a0b46d6dcb2bddda7000ad1696d27998a16a1e752dc75096",
                @"{
                    MaybeStrings = ()
                }",
                @"{
                    ""MaybeStrings"": []
                }");
            AddSample(
                new NullableStringMap(new Dataset() { { 4, "bacon" } }),
                "89775637108ae9b95ed59ea3b353f87434c639a0d1721404155b95273c97ccd2",
                @"{
                    MaybeStrings = (
                        4 -> 'bacon'
                    )
                }",
                @"{
                    ""MaybeStrings"": [{
                        ""k"":4,
                        ""v"":""bacon""
                    }]
                }");
            AddSample(
                new NullableStringMap(new Dataset() { { 4, null } }),
                "e3f9e9f724c3b1632a6e370f3b95352dafcbbe762aed9d893352408aee36f474",
                @"{
                    MaybeStrings = (
                        4 -> null
                    )
                }",
                @"{
                    ""MaybeStrings"": [{
                        ""k"":4,
                        ""v"":null
                    }]
                }");
            AddSample(
                new NullableStringMap(new Dataset() { { 7, "eggs" }, { 6, "sausage" } }),
                "3feff5c1c53ed32987d19deecd60d2b4a4c362256c0ab88ccf5f5066d75954c6",
                @"{
                    MaybeStrings = (
                        6 -> 'sausage',
                        7 -> 'eggs'
                    )
                }",
                @"{
                    ""MaybeStrings"": [{
                        ""k"":6,
                        ""v"":""sausage""
                    },{
                        ""k"":7,
                        ""v"":""eggs""
                    }]
                }");
            AddSample(
                new NullableStringMap(new Dataset() { { 7, null }, { 6, "sausage" } }),
                "7017db89709bf73851487110ea28218b0233d0756c5bbe93be0399bf2315746e",
                @"{
                    MaybeStrings = (
                        6 -> 'sausage',
                        7 -> null
                    )
                }",
                @"{
                    ""MaybeStrings"": [{
                        ""k"":6,
                        ""v"":""sausage""
                    },{
                        ""k"":7,
                        ""v"":null
                    }]
                }");

            AddInvalidConstructions(
                () => { var a = new NullableStringMap(null); },
                () => { var a = new NullableStringMap.Builder(null); }
            );
        }

        [Test]
        public override void Builder()
        {
            var Ka = 1;
            var Kb = 4;
            var Kc = 8;
            var Kd = 15;
            var Va = "bruh";
            var Vb = "brah";
            var Vc = (string)null;
            var Vd = "breh";

            var sut = new NullableStringMap(new Dataset() { { Ka, Va }, { Kb, Vb } }).ToBuilder();
            sut.SetMaybeString(Ka, Vc);
            sut.RemoveMaybeString(Kb);
            Assert.True(sut.ContainsMaybeStringKey(Ka));
            Assert.False(sut.ContainsMaybeStringKey(Kb));
            Assert.AreEqual(1, sut.CountMaybeStrings);
            Assert.AreEqual(Vc, sut.GetMaybeString(Ka));
            Assert.AreEqual(new[] { Ka }, sut.MaybeStringKeys);
            Assert.AreEqual(new[] { new KV(Ka, Vc) }, sut.MaybeStringValues);

            sut.ClearMaybeStrings();
            Assert.AreEqual(0, sut.CountMaybeStrings);

            sut.SetMaybeString(Kc, Vc);
            sut.SetMaybeString(Kd, Vd);
            Assert.AreEqual(
                new NullableStringMap.Builder(new Dataset() { { Kc, Vc }, { Kd, Vd } }).Build(),
                sut.Build());
        }
    }
}
