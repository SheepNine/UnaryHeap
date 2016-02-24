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
    }
}
