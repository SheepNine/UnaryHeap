using System;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.Misc;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class LinearMapping2DTests
    {
        [Fact]
        public void NonsingularResult()
        {
            var src1 = new Point2D(1, 3);
            var src2 = new Point2D(2, -2);
            var dst1 = new Point2D(0, 3);
            var dst2 = new Point2D(1, 1);

            // The order that the points are specified should not affect the results
            var sut1 = LinearMapping.From(src1, src2).Onto(dst1, dst2);
            var sut2 = LinearMapping.From(src2, src1).Onto(dst2, dst1);

            Assert.Equal(dst1, sut1 * src1);
            Assert.Equal(dst2, sut1 * src2);
            Assert.Equal(dst1, sut2 * src1);
            Assert.Equal(dst2, sut2 * src2);
            Assert.Equal(Point2D.Origin, sut1 * Point2D.Origin);
            Assert.Equal(Point2D.Origin, sut2 * Point2D.Origin);

            var sutInv = sut1.ComputeInverse();

            Assert.Equal(src1, sutInv * dst1);
            Assert.Equal(src2, sutInv * dst2);
        }

        [Fact]
        public void SingularResult()
        {
            var src1 = new Point2D(1, 3);
            var src2 = new Point2D(2, -2);
            var dst1 = new Point2D(2, 2);
            var dst2 = new Point2D(2, 2);

            // The order that the points are specified should not affect the results
            var sut1 = LinearMapping.From(src1, src2).Onto(dst1, dst2);
            var sut2 = LinearMapping.From(src2, src1).Onto(dst2, dst1);

            Assert.Equal(dst1, sut1 * src1);
            Assert.Equal(dst2, sut1 * src2);
            Assert.Equal(dst1, sut2 * src1);
            Assert.Equal(dst2, sut2 * src2);
            Assert.Equal(Point2D.Origin, sut1 * Point2D.Origin);
            Assert.Equal(Point2D.Origin, sut2 * Point2D.Origin);

            Assert.Throws<InvalidOperationException>(() => { sut1.ComputeInverse(); });
        }

        [Fact]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("src1",
                () => { LinearMapping.From(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("src2",
                () => { LinearMapping.From(Point2D.Origin, null); });

            var from = LinearMapping.From(new Point2D(1, 0), new Point2D(0, 1));

            Assert.Throws<ArgumentNullException>("dst1",
                () => { from.Onto(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("dst2",
                () => { from.Onto(Point2D.Origin, null); });

            Assert.Equal("Source points are linearly dependent; cannot invert.",
                Assert.Throws<ArgumentException>(
                () => { LinearMapping.From(new Point2D(1, 1), new Point2D(2, 2)); })
                .Message);
        }

#if INCLUDE_WORK_IN_PROGRESS
        [Fact]
        public void ThreeDimensions()
        {
            var src1 = new Point3D(1, 2, 3);
            var src2 = new Point3D(4, 5, 6);
            var src3 = new Point3D(7, 8, 10);

            var dst1 = new Point3D(1, 1, 3);
            var dst2 = new Point3D(2, 4, 3);
            var dst3 = new Point3D(-1, 0, 1);

            var suts = new[]
            {
                LinearMapping.From(src1, src2, src3).To(dst1, dst2, dst3),
                LinearMapping.From(src1, src3, src2).To(dst1, dst3, dst2),
                LinearMapping.From(src3, src1, src2).To(dst3, dst1, dst2),
                LinearMapping.From(src3, src2, src1).To(dst3, dst2, dst1),
                LinearMapping.From(src2, src3, src1).To(dst2, dst3, dst1),
                LinearMapping.From(src2, src1, src3).To(dst2, dst1, dst3),
            };

            foreach (var sut in suts)
            {
                Assert.Equal(dst1, sut * src1);
                Assert.Equal(dst2, sut * src2);
                Assert.Equal(dst3, sut * src3);
            }
        }
#endif
    }
}
