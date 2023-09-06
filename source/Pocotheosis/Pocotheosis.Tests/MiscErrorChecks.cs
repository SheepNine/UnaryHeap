using GeneratedTestPocos;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class MiscErrorChecks
    {
        [Test]
        public void DeserializeUnknownId()
        {
            var data = new byte[] { 0, 1, 0, 0 };

            Assert.Throws<InvalidDataException>(() =>
            {
                using (var stream = new MemoryStream(data))
                    Poco.DeserializeWithId(stream);
            });
        }

        [Test]
        public void DeserializeIncorrectId()
        {
            var data = new byte[] { 1, 0, 0, 0 };

            Assert.Throws<InvalidDataException>(() =>
            {
                using (var stream = new MemoryStream(data))
                    Poco.DeserializeNullable<PrimitiveValue>(2, (stream) => null, stream);
            });
        }

        [Test]
        public void DeserializeInvalidBoolean()
        {
            byte notABool = 0x80;
            var data = new byte[] { 1, 0, 0, 0, notABool, 1 };

            Assert.Throws<InvalidDataException>(() =>
            {
                using (var stream = new MemoryStream(data))
                    PrimitiveMap.Deserialize(stream);
            });
        }

        [Test]
        public void DeserializeIncompleteStream()
        {
            byte[] data;
            using (var buffer = new MemoryStream())
            {
                new NullableClassValue(new PrimitiveValue(175)).Serialize(buffer);
                data = buffer.ToArray();
            }

            foreach (var length in Enumerable.Range(0, data.Length))
            {
                Assert.Throws<InvalidDataException>(() =>
                {
                    using (var stream = new MemoryStream(data, 0, length))
                        NullableClassValue.Deserialize(stream);
                });
            }
        }
    }
}
