using System;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.Misc;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class AffineMapping2DTests
    {
        [Fact]
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
                Assert.Equal(dst1, (sut * src1.Homogenized()).Dehomogenized());
                Assert.Equal(dst2, (sut * src2.Homogenized()).Dehomogenized());
                Assert.Equal(dst3, (sut * src3.Homogenized()).Dehomogenized());
            }
        }

        [Fact]
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
                Assert.Equal(dst1, (sut * src1.Homogenized()).Dehomogenized());
                Assert.Equal(dst2, (sut * src2.Homogenized()).Dehomogenized());
                Assert.Equal(dst3, (sut * src3.Homogenized()).Dehomogenized());
            }
        }

        [Fact]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("src1",
                () => { AffineMapping.From(null, Point2D.Origin, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("src2",
                () => { AffineMapping.From(Point2D.Origin, null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("src3",
                () => { AffineMapping.From(Point2D.Origin, Point2D.Origin, null); });

            var sut = AffineMapping.From(
                new Point2D(1, 0), new Point2D(0, 1), new Point2D(0, 0));

            Assert.Throws<ArgumentNullException>("dst1",
                () => { sut.Onto(null, Point2D.Origin, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("dst2",
                () => { sut.Onto(Point2D.Origin, null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("dst3",
                () => { sut.Onto(Point2D.Origin, Point2D.Origin, null); });
        }
    }
}