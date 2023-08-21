using Pocotheosis.Tests.Pocos;

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
                () => { var a = new ClassValue(null); }
            );
        }
    }
}
