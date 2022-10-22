using NUnit.Framework;
using System;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class LinearMapping2DTests
    {
        [Test]
        public void NonsingularResult()
        {
            var src1 = new Point2D(1, 3);
            var src2 = new Point2D(2, -2);
            var dst1 = new Point2D(0, 3);
            var dst2 = new Point2D(1, 1);

            // The order that the points are specified should not affect the results
            var sut1 = LinearMapping.From(src1, src2).Onto(dst1, dst2);
            var sut2 = LinearMapping.From(src2, src1).Onto(dst2, dst1);

            Assert.AreEqual(dst1, sut1 * src1);
            Assert.AreEqual(dst2, sut1 * src2);
            Assert.AreEqual(dst1, sut2 * src1);
            Assert.AreEqual(dst2, sut2 * src2);
            Assert.AreEqual(Point2D.Origin, sut1 * Point2D.Origin);
            Assert.AreEqual(Point2D.Origin, sut2 * Point2D.Origin);

            var sutInv = sut1.ComputeInverse();

            Assert.AreEqual(src1, sutInv * dst1);
            Assert.AreEqual(src2, sutInv * dst2);
        }

        [Test]
        public void SingularResult()
        {
            var src1 = new Point2D(1, 3);
            var src2 = new Point2D(2, -2);
            var dst1 = new Point2D(2, 2);
            var dst2 = new Point2D(2, 2);

            // The order that the points are specified should not affect the results
            var sut1 = LinearMapping.From(src1, src2).Onto(dst1, dst2);
            var sut2 = LinearMapping.From(src2, src1).Onto(dst2, dst1);

            Assert.AreEqual(dst1, sut1 * src1);
            Assert.AreEqual(dst2, sut1 * src2);
            Assert.AreEqual(dst1, sut2 * src1);
            Assert.AreEqual(dst2, sut2 * src2);
            Assert.AreEqual(Point2D.Origin, sut1 * Point2D.Origin);
            Assert.AreEqual(Point2D.Origin, sut2 * Point2D.Origin);

            Assert.Throws<InvalidOperationException>(() => { sut1.ComputeInverse(); });
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { LinearMapping.From(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { LinearMapping.From(Point2D.Origin, null); });

            var from = LinearMapping.From(new Point2D(1, 0), new Point2D(0, 1));

            Assert.Throws<ArgumentNullException>(
                () => { from.Onto(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { from.Onto(Point2D.Origin, null); });

            Assert.That(
                Assert.Throws<ArgumentException>(
                    () => { LinearMapping.From(new Point2D(1, 1), new Point2D(2, 2)); })
                .Message.StartsWith(
                    "Source points are linearly dependent; cannot invert."));
        }
    }
}
