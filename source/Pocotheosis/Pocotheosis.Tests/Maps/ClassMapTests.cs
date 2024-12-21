using NUnit.Framework;
using GeneratedTestPocos;
using System.Linq;
using Dataset = System.Collections.Generic.Dictionary
        <string, GeneratedTestPocos.PrimitiveValue>;
using KV = System.Collections.Generic.KeyValuePair
        <string, GeneratedTestPocos.PrimitiveValue>;

namespace Pocotheosis.Tests.Maps
{
    public class ClassMapTests : PocoTestFixture<ClassMap>
    {
        public ClassMapTests()
        {
            AddSample(
                new ClassMap(new Dataset()),
                "8b1fde09592443f1b32b4e08172bd374357872bdd6e048bc3ef540c32c6cc22c",
                @"{
                    Pocos = ()
                }",
                @"{
                    ""Pocos"": {}
                }");
            AddSample(
                new ClassMap(new Dataset() { { "bacon", P(25) } }),
                "9571ec336b0e28177be7517c7578d57e5ab705001e0f692058ac0abfa2b8de57",
                @"{
                    Pocos = (
                        'bacon' -> {
                            Primitive = 25
                        }
                    )
                }",
                @"{
                    ""Pocos"": {
                        ""bacon"": {
                            ""Primitive"": 25
                        }
                    }
                }");
            AddSample(
                new ClassMap(new Dataset() {
                    { "eggs", P(2) },
                    { "sausage", P(99) }
                }),
                "e7596ec3155e54bcb093055a5a76d4ea6a4ef624ea69438add44bcf4247117da",
                @"{
                    Pocos = (
                        'eggs' -> {
                            Primitive = 2
                        },
                        'sausage' -> {
                            Primitive = 99
                        }
                    )
                }",
                @"{
                    ""Pocos"": {
                        ""eggs"": {
                            ""Primitive"": 2
                        },
                        ""sausage"": {
                            ""Primitive"": 99
                        }
                    }
                }");

            AddInvalidConstructions(
                () => { var a = new ClassMap(null); },
                () => { var a = new ClassMap.Builder(null); },
                () => { var a = new ClassMap(new Dataset() { { "null", null } }); },
                () => { var a = new ClassMap.Builder(new Dataset() { { "null", null } }); },
                () => { var a = new ClassMap(new Dataset() { { null, P(4) } }); },
                () => { var a = new ClassMap.Builder(new Dataset() { { null, P(4) } }); },
                () => { new ClassMap.Builder(new Dataset()).SetPoco("null", null); },
                () => { new ClassMap.Builder(new Dataset()).SetPoco(null, P(4)); }
            );
        }

        [Test]
        public override void Builder()
        {
            var Ka = "bruh";
            var Kb = "BRUH";
            var Kc = "breh";
            var Kd = "bruuuu";
            var Va = P(5);
            var Vb = P(18);
            var Vc = P(99);
            var Vd = P(2);

            var sut = new ClassMap(new Dataset() { { Ka, Va }, { Kb, Vb } }).ToBuilder();
            sut.SetPoco(Ka, Vc);
            sut.RemovePoco(Kb);
            Assert.True(sut.ContainsPocoKey(Ka));
            Assert.False(sut.ContainsPocoKey(Kb));
            Assert.AreEqual(1, sut.CountPocos);
            Assert.AreEqual(Vc, sut.GetPoco(Ka).Build());
            Assert.AreEqual(new[] { Ka }, sut.PocoKeys);
            Assert.AreEqual(new[] { new KV(Ka, Vc) },
                sut.PocoValues.Select(kv => new KV(kv.Key, kv.Value.Build())));

            sut.ClearPocos();
            Assert.AreEqual(0, sut.CountPocos);

            sut.SetPoco(Kc, Vc);
            sut.SetPoco(Kd, Vd);
            Assert.AreEqual(
                new ClassMap.Builder(new Dataset() { { Kc, Vc }, { Kd, Vd } }).Build(),
                sut.Build());
        }
    }
}
