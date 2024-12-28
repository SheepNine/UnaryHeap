using NUnit.Framework;
using UnaryHeap.DataType;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class IndexAssignerTests
    {
        [Test]
        public void IndexAssignment()
        {
            var sut = new IndexAssigner<Rational>();
            Assert.AreEqual(0, sut.Count);

            Assert.AreEqual(0, sut.GetIndex(3));
            Assert.AreEqual(1, sut.Count);

            Assert.AreEqual(1, sut.GetIndex(8));
            Assert.AreEqual(2, sut.Count);

            Assert.AreEqual(0, sut.GetIndex(3));
            Assert.AreEqual(2, sut.Count);

            Assert.AreEqual((Rational)3, sut[0]);
            Assert.AreEqual((Rational)8, sut[1]);
        }
    }
}
