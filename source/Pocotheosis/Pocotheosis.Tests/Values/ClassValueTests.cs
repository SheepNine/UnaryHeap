using NUnit.Framework;
using GeneratedTestPocos;

namespace Pocotheosis.Tests.Values
{
    internal class ClassValueTests : PocoTestFixture<ClassValue>
    {
        public ClassValueTests()
        {
            AddSample(
                new ClassValue(new PrimitiveValue(1)),
                "581bd447907b3224b5bdb31111cdd21a453f5a576166f6a81b3442136e2d1e8b",
                @"{
                    Poco = {
                        Primitive = 1
                    }
                }",
                @"{
                    ""Poco"": {
                        ""Primitive"": 1
                    }
                }");
            AddSample(
                new ClassValue(new PrimitiveValue(20)),
                "87ad3957324605f790a47279a5a03a25ea9c13665e2de04c4ca424a3a94f9a1f",
                @"{
                    Poco = {
                        Primitive = 20
                    }
                }",
                @"{
                    ""Poco"": {
                        ""Primitive"": 20
                    }
                }");

            AddInvalidConstructions(
                () => { var a = new ClassValue(null); },
                () => { var a = new ClassValue.Builder(null); },
                () => { new ClassValue.Builder(new PrimitiveValue(0)).WithPoco(null); },
                () => { ReadTypedFromJson("{\"Poco\":null}", false); }
            );
        }

        [Test]
        public override void Builder()
        {
            var sut = new ClassValue(new PrimitiveValue(8)).ToBuilder();
            Assert.AreEqual(8, sut.Poco.Primitive);
            Assert.AreEqual(
                sut.WithPoco(new PrimitiveValue(11)).Build(),
                new ClassValue.Builder(new PrimitiveValue(11)).Build());
        }
    }
}
