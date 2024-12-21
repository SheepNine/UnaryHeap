using NUnit.Framework;
using GeneratedTestPocos;
using Dataset = System.Collections.Generic.Dictionary
    <GeneratedTestPocos.TrueBool, string>;
using KV = System.Collections.Generic.KeyValuePair
    <GeneratedTestPocos.TrueBool, string>;

namespace Pocotheosis.Tests.Maps
{
    public class StringMapTests : PocoTestFixture<StringMap>
    {
        public StringMapTests()
        {
            AddSample(
                new StringMap(new Dataset()),
                "8947bc952889a356fa03167e8a2e721866c8df002bab9b5d400991a38532d3c1",
                @"{
                    Strs = ()
                }",
                @"{
                    ""Strs"": {}
                }");
            AddSample(
                new StringMap(new Dataset() { { Tru, "bacon" } }),
                "8d4a0f0ff0973b00c240f38edc8ad8cfee6c230488b6a1b04a9fdeed699fdfbe",
                @"{
                    Strs = (
                        True -> 'bacon'
                    )
                }",
                @"{
                    ""Strs"": {
                        ""True"": ""bacon""
                    }
                }");
            AddSample(
                new StringMap(new Dataset() {
                    { Tru, "eggs" },
                    { FNF, "sausage" }
                }),
                "b58d78377b088eab88cac3cffd796491da4f51f7500a2d9e660be1226f8f075c",
                @"{
                    Strs = (
                        True -> 'eggs',
                        FileNotFound -> 'sausage'
                    )
                }",
                @"{
                    ""Strs"": {
                        ""True"": ""eggs"",
                        ""FileNotFound"": ""sausage""
                    }
                }");

            AddInvalidConstructions(
                () => { var a = new StringMap(null); },
                () => { var a = new StringMap(new Dataset() { { Tru, null } }); },
                () => { var a = new StringMap(new Dataset() { { Tru, null } }); },
                () => { var a = new StringMap.Builder(new Dataset() { { Tru, null } }); },
                () => { new StringMap.Builder(new Dataset()).SetStr(Tru, null); }
            );
        }

        [Test]
        public override void Builder()
        {
            var Ka = Fls;
            var Kb = Tru;
            var Kc = FNF;
            var Kd = Fls;
            var Va = "bruh";
            var Vb = "brah";
            var Vc = "bryh";
            var Vd = "breh";

            var sut = new StringMap(new Dataset() { { Ka, Va }, { Kb, Vb } }).ToBuilder();
            sut.SetStr(Ka, Vc);
            sut.RemoveStr(Kb);
            Assert.True(sut.ContainsStrKey(Ka));
            Assert.False(sut.ContainsStrKey(Kb));
            Assert.AreEqual(1, sut.CountStrs);
            Assert.AreEqual(Vc, sut.GetStr(Ka));
            Assert.AreEqual(new[] { Ka }, sut.StrKeys);
            Assert.AreEqual(new[] { new KV(Ka, Vc) }, sut.StrValues);

            sut.ClearStrs();
            Assert.AreEqual(0, sut.CountStrs);

            sut.SetStr(Kc, Vc);
            sut.SetStr(Kd, Vd);
            Assert.AreEqual(
                new StringMap.Builder(new Dataset() { { Kc, Vc }, { Kd, Vd } }).Build(),
                sut.Build());
        }
    }
}
