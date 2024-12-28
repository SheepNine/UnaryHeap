using NUnit.Framework;
using System;
using System.Linq;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class AffineMapping3DTests
    {
        [Test]
        public void NonSingularResult()
        {
            AffineMappingTestCase(new[]
            {
                new Point3D(1, -1, 2), new Point3D(4, 2, 1),
                new Point3D(4, 5, -4), new Point3D(1, 2, 1),
            }, new[]
            {
                new Point3D(1, 3, 1), new Point3D(2, 4, 5),
                new Point3D(5, 1, -1), new Point3D(6, 3, 3),
            });
        }

        [Test]
        public void SingularResult()
        {
            AffineMappingTestCase(new[]
            {
                new Point3D(1, -1, 2), new Point3D(4, 2, 1),
                new Point3D(4, 5, -4), new Point3D(1, 2, 1),
            }, new[]
            {
                new Point3D(1, 3, 1), new Point3D(1, 3, 1),
                new Point3D(1, 3, 1), new Point3D(1, 3, 1),
            });
        }

        static void AffineMappingTestCase(Point3D[] sources, Point3D[] targets)
        {
            var permutations = TestUtils.PermuteIndices(4).ToList();
            foreach (var permutation in permutations)
            {
                var sut = AffineMapping.From(
                    sources[permutation[0]], sources[permutation[1]],
                    sources[permutation[2]], sources[permutation[3]]
                ).Onto(
                    targets[permutation[0]], targets[permutation[1]],
                    targets[permutation[2]], targets[permutation[3]]
                );

                foreach (var i in Enumerable.Range(0, 4))
                    Assert.AreEqual(targets[i], Map(sut, sources[i]));
            }
        }

        static Point3D Map(Matrix4D transform, Point3D point)
        {
            return (transform * point.Homogenized()).Dehomogenized();
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            var point = Point3D.Origin;
            var from = AffineMapping.From(
                Point3D.Origin,
                new Point3D(1, 0, 0),
                new Point3D(0, 1, 0),
                new Point3D(0, 0, 1)
            );

            TestUtils.NullChecks(new()
            {
                { typeof(ArgumentNullException), new TestDelegate[] {
                    () => { _ = AffineMapping.From(null, point, point, point); },
                    () => { _ = AffineMapping.From(point, null, point, point); },
                    () => { _ = AffineMapping.From(point, point, null, point); },
                    () => { _ = AffineMapping.From(point, point, point, null); },
                    () => { from.Onto(null, point, point, point); },
                    () => { from.Onto(point, null, point, point); },
                    () => { from.Onto(point, point, null, point); },
                    () => { from.Onto(point, point, point, null); },
                }}
            });

            Assert.AreEqual("Source points are linearly dependent; cannot invert.",
                Assert.Throws<ArgumentException>(
                () =>
                {
                    AffineMapping.From(new Point3D(1, 2, 3), new Point3D(4, 5, 6),
                        new Point3D(7, 8, 9), new Point3D(10, 11, 12));
                }
                ).Message);
        }
    }
}
