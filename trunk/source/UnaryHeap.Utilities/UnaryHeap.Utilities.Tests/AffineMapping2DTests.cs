#if INCLUDE_WORK_IN_PROGRESS

using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.Misc;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class AffineMapping2DTests
    {
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
    }
}

#endif