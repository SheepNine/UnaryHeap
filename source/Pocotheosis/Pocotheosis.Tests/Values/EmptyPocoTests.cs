using Newtonsoft.Json;
using NUnit.Framework;
using GeneratedTestPocos;
using System.IO;

namespace Pocotheosis.Tests.Values
{
    public class EmptyPocoTests : PocoTestFixture<EmptyPoco>
    {
        public EmptyPocoTests()
        {
            AddSample(
                new EmptyPoco(),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{ }",
                @"{}");

            NoInvalidConstructions();
        }

        [Test]
        public void DefaultJsonDeserializeValue()
        {
            // Explicity check that the 'isNullable' value of these methods defaults to false
            using (var jsonReader = new JsonTextReader(new StringReader("{}")))
                Assert.IsNotNull(PocoJson.DeserializeEmptyPoco(jsonReader));
            using (var jsonReader = new JsonTextReader(new StringReader("null")))
                Assert.Throws<InvalidDataException>(
                    () => PocoJson.DeserializeEmptyPoco(jsonReader));
        }
    }
}
