using NUnit.Framework;
using System;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class AffineMapping1DTests
    {
        [Test]
        public void NonSingularResult()
        {
            Rational src1 = 2;
            Rational src2 = 3;
            Rational dst1 = 6;
            Rational dst2 = 4;

            // The order that the points are specified should not affect the results
            var sut1 = AffineMapping.From(src1, src2).Onto(dst1, dst2);
            var sut2 = AffineMapping.From(src2, src1).Onto(dst2, dst1);

            Assert.AreEqual(dst1, (sut1 * src1.Homogenized()).Dehomogenized());
            Assert.AreEqual(dst2, (sut1 * src2.Homogenized()).Dehomogenized());
            Assert.AreEqual(dst1, (sut2 * src1.Homogenized()).Dehomogenized());
            Assert.AreEqual(dst2, (sut2 * src2.Homogenized()).Dehomogenized());

            var sutInv = sut1.ComputeInverse();

            Assert.AreEqual(src1, (sutInv * dst1.Homogenized()).Dehomogenized());
            Assert.AreEqual(src2, (sutInv * dst2.Homogenized()).Dehomogenized());
        }

        [Test]
        public void SingularResult()
        {
            Rational src1 = 2;
            Rational src2 = 3;
            Rational dst1 = 5;
            Rational dst2 = 5;

            // The order that the points are specified should not affect the results
            var sut1 = AffineMapping.From(src1, src2).Onto(dst1, dst2);
            var sut2 = AffineMapping.From(src2, src1).Onto(dst2, dst1);

            Assert.AreEqual(dst1, (sut1 * src1.Homogenized()).Dehomogenized());
            Assert.AreEqual(dst2, (sut1 * src2.Homogenized()).Dehomogenized());
            Assert.AreEqual(dst1, (sut2 * src1.Homogenized()).Dehomogenized());
            Assert.AreEqual(dst2, (sut2 * src2.Homogenized()).Dehomogenized());

            Assert.Throws<InvalidOperationException>(() => { sut1.ComputeInverse(); });
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(() => { AffineMapping.From(null, 0); });
            Assert.Throws<ArgumentNullException>(() => { AffineMapping.From(0, null); });

            var from = AffineMapping.From(0, 1);

            Assert.Throws<ArgumentNullException>(() => { from.Onto(null, 0); });
            Assert.Throws<ArgumentNullException>(() => { from.Onto(0, null); });

            Assert.AreEqual("Source points are linearly dependent; cannot invert.",
                Assert.Throws<ArgumentException>(
                () => { AffineMapping.From(1, 1); })
                .Message);
        }
    }
}
