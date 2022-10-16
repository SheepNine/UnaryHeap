using System;
using UnaryHeap.DataType;
using UnaryHeap.Utilities.Misc;
using NUnit.Framework;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class AffineMapping2DTests
    {
        [Test]
        public void NonSingularResult()
        {
            var src1 = new Point2D(1, 2);
            var src2 = new Point2D(4, 5);
            var src3 = new Point2D(7, 9);

            var dst1 = new Point2D(1, 1);
            var dst2 = new Point2D(2, 4);
            var dst3 = new Point2D(-1, 0);

            var suts = new[]
            {
                AffineMapping.From(src1, src2, src3).Onto(dst1, dst2, dst3),
                AffineMapping.From(src1, src3, src2).Onto(dst1, dst3, dst2),
                AffineMapping.From(src3, src1, src2).Onto(dst3, dst1, dst2),
                AffineMapping.From(src3, src2, src1).Onto(dst3, dst2, dst1),
                AffineMapping.From(src2, src3, src1).Onto(dst2, dst3, dst1),
                AffineMapping.From(src2, src1, src3).Onto(dst2, dst1, dst3),
            };

            foreach (var sut in suts)
            {
                Assert.AreEqual(dst1, (sut * src1.Homogenized()).Dehomogenized());
                Assert.AreEqual(dst2, (sut * src2.Homogenized()).Dehomogenized());
                Assert.AreEqual(dst3, (sut * src3.Homogenized()).Dehomogenized());
            }
        }

        [Test]
        public void SingularResult()
        {
            var src1 = new Point2D(1, 2);
            var src2 = new Point2D(4, 5);
            var src3 = new Point2D(7, 9);

            var dst1 = new Point2D(2, 2);
            var dst2 = new Point2D(2, 2);
            var dst3 = new Point2D(2, 2);

            var suts = new[]
            {
                AffineMapping.From(src1, src2, src3).Onto(dst1, dst2, dst3),
                AffineMapping.From(src1, src3, src2).Onto(dst1, dst3, dst2),
                AffineMapping.From(src3, src1, src2).Onto(dst3, dst1, dst2),
                AffineMapping.From(src3, src2, src1).Onto(dst3, dst2, dst1),
                AffineMapping.From(src2, src3, src1).Onto(dst2, dst3, dst1),
                AffineMapping.From(src2, src1, src3).Onto(dst2, dst1, dst3),
            };

            foreach (var sut in suts)
            {
                Assert.AreEqual(dst1, (sut * src1.Homogenized()).Dehomogenized());
                Assert.AreEqual(dst2, (sut * src2.Homogenized()).Dehomogenized());
                Assert.AreEqual(dst3, (sut * src3.Homogenized()).Dehomogenized());
            }
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { AffineMapping.From(null, Point2D.Origin, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { AffineMapping.From(Point2D.Origin, null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { AffineMapping.From(Point2D.Origin, Point2D.Origin, null); });

            var sut = AffineMapping.From(
                new Point2D(1, 0), new Point2D(0, 1), new Point2D(0, 0));

            Assert.Throws<ArgumentNullException>(
                () => { sut.Onto(null, Point2D.Origin, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.Onto(Point2D.Origin, null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.Onto(Point2D.Origin, Point2D.Origin, null); });
        }
    }
}