#if INCLUDE_WORK_IN_PROGRESS

using UnaryHeap.Utilities.D3;
using UnaryHeap.Utilities.Misc;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class LinearMapping3DTests
    {
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
    }
}

#endif