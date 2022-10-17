using NUnit.Framework;
using System;
using UnaryHeap.DataType;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class LinearMapping3DTests
    {
        [Test]
        public void NonSingularResult()
        {
            var src1 = new Point3D(1, 2, 3);
            var src2 = new Point3D(4, 5, 6);
            var src3 = new Point3D(7, 8, 10);

            var dst1 = new Point3D(1, 1, 3);
            var dst2 = new Point3D(2, 4, 3);
            var dst3 = new Point3D(-1, 0, 1);

            var suts = new[]
            {
                LinearMapping.From(src1, src2, src3).Onto(dst1, dst2, dst3),
                LinearMapping.From(src1, src3, src2).Onto(dst1, dst3, dst2),
                LinearMapping.From(src3, src1, src2).Onto(dst3, dst1, dst2),
                LinearMapping.From(src3, src2, src1).Onto(dst3, dst2, dst1),
                LinearMapping.From(src2, src3, src1).Onto(dst2, dst3, dst1),
                LinearMapping.From(src2, src1, src3).Onto(dst2, dst1, dst3),
            };

            foreach (var sut in suts)
            {
                Assert.AreEqual(dst1, sut * src1);
                Assert.AreEqual(dst2, sut * src2);
                Assert.AreEqual(dst3, sut * src3);
            }
        }

        [Test]
        public void SingularResult()
        {
            var src1 = new Point3D(1, 2, 3);
            var src2 = new Point3D(4, 5, 6);
            var src3 = new Point3D(7, 8, 10);

            var dst1 = new Point3D(1, 2, 2);
            var dst2 = new Point3D(1, 2, 2);
            var dst3 = new Point3D(1, 2, 2);

            var suts = new[]
            {
                LinearMapping.From(src1, src2, src3).Onto(dst1, dst2, dst3),
                LinearMapping.From(src1, src3, src2).Onto(dst1, dst3, dst2),
                LinearMapping.From(src3, src1, src2).Onto(dst3, dst1, dst2),
                LinearMapping.From(src3, src2, src1).Onto(dst3, dst2, dst1),
                LinearMapping.From(src2, src3, src1).Onto(dst2, dst3, dst1),
                LinearMapping.From(src2, src1, src3).Onto(dst2, dst1, dst3),
            };

            foreach (var sut in suts)
            {
                Assert.AreEqual(dst1, sut * src1);
                Assert.AreEqual(dst2, sut * src2);
                Assert.AreEqual(dst3, sut * src3);
            }
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { LinearMapping.From(null, Point3D.Origin, Point3D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { LinearMapping.From(Point3D.Origin, null, Point3D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { LinearMapping.From(Point3D.Origin, Point3D.Origin, null); });

            var sut = LinearMapping.From(
                new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1));

            Assert.Throws<ArgumentNullException>(
                () => { sut.Onto(null, Point3D.Origin, Point3D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.Onto(Point3D.Origin, null, Point3D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.Onto(Point3D.Origin, Point3D.Origin, null); });
        }
    }
}