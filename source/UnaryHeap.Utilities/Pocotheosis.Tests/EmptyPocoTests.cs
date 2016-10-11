using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class EmptyPocoTests
    {
        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new EmptyPoco());
            Assert.AreEqual(new EmptyPoco(), new EmptyPoco());
        }
    }
}
