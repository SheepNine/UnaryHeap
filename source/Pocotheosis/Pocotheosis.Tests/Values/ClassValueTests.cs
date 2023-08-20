using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    internal class ClassValueTests : PocoTestFixture<ClassValue>
    {
        public ClassValueTests()
        {
            AddSample(
                new ClassValue(new PrimitiveValue(1)),
                "4bf5122f344554c53bde2ebb8cd2b7e3d1600ad631c385a5d7cce23c7785459a",
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
                "83891d7fe85c33e52c8b4e5814c92fb6a3b9467299200538a6babaa8b452d879",
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
        }
    }
}
