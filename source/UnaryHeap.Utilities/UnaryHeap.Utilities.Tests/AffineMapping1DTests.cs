using System;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.Misc;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class AffineMapping1DTests
    {
        [Fact]
        public void NonSingularResult()
        {
            Rational src1 = 2;
            Rational src2 = 3;
            Rational dst1 = 6;
            Rational dst2 = 4;

            // The order that the points are specified should not affect the results
            var sut1 = AffineMapping.From(src1, src2).Onto(dst1, dst2);
            var sut2 = AffineMapping.From(src2, src1).Onto(dst2, dst1);

            Assert.Equal(dst1, (sut1 * src1.Homogenized()).Dehomogenized());
            Assert.Equal(dst2, (sut1 * src2.Homogenized()).Dehomogenized());
            Assert.Equal(dst1, (sut2 * src1.Homogenized()).Dehomogenized());
            Assert.Equal(dst2, (sut2 * src2.Homogenized()).Dehomogenized());

            var sutInv = sut1.ComputeInverse();

            Assert.Equal(src1, (sutInv * dst1.Homogenized()).Dehomogenized());
            Assert.Equal(src2, (sutInv * dst2.Homogenized()).Dehomogenized());
        }

        [Fact]
        public void SingularResult()
        {
            Rational src1 = 2;
            Rational src2 = 3;
            Rational dst1 = 5;
            Rational dst2 = 5;

            // The order that the points are specified should not affect the results
            var sut1 = AffineMapping.From(src1, src2).Onto(dst1, dst2);
            var sut2 = AffineMapping.From(src2, src1).Onto(dst2, dst1);

            Assert.Equal(dst1, (sut1 * src1.Homogenized()).Dehomogenized());
            Assert.Equal(dst2, (sut1 * src2.Homogenized()).Dehomogenized());
            Assert.Equal(dst1, (sut2 * src1.Homogenized()).Dehomogenized());
            Assert.Equal(dst2, (sut2 * src2.Homogenized()).Dehomogenized());

            Assert.Throws<InvalidOperationException>(() => { sut1.ComputeInverse(); });
        }

        [Fact]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("src1",
                () => { AffineMapping.From(null, 0); });
            Assert.Throws<ArgumentNullException>("src2",
                () => { AffineMapping.From(0, null); });

            var from = AffineMapping.From(0, 1);

            Assert.Throws<ArgumentNullException>("dst1",
                () => { from.Onto(null, 0); });
            Assert.Throws<ArgumentNullException>("dst2",
                () => { from.Onto(0, null); });

            Assert.Equal("Source points are linearly dependent; cannot invert.",
                Assert.Throws<ArgumentException>(
                () => { AffineMapping.From(1, 1); })
                .Message);
        }

#if INCLUDE_WORK_IN_PROGRESS
        [Fact]
        public void TwoDimensions()
        {
            var src1 = new Point2D(1, 2);
            var src2 = new Point2D(4, 5);
            var src3 = new Point2D(7, 9);

            var dst1 = new Point2D(1, 1);
            var dst2 = new Point2D(2, 4);
            var dst3 = new Point2D(-1, 0);

            var suts = new[]
            {
                AffineMapping.From(src1, src2, src3).To(dst1, dst2, dst3),
                AffineMapping.From(src1, src3, src2).To(dst1, dst3, dst2),
                AffineMapping.From(src3, src1, src2).To(dst3, dst1, dst2),
                AffineMapping.From(src3, src2, src1).To(dst3, dst2, dst1),
                AffineMapping.From(src2, src3, src1).To(dst2, dst3, dst1),
                AffineMapping.From(src2, src1, src3).To(dst2, dst1, dst3),
            };

            foreach (var sut in suts)
            {
                Assert.Equal(dst1, (sut * src1.Homogenized()).Dehomogenized());
                Assert.Equal(dst2, (sut * src2.Homogenized()).Dehomogenized());
                Assert.Equal(dst3, (sut * src3.Homogenized()).Dehomogenized());
            }
        }
#endif
    }
}
