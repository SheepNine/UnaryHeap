using NUnit.Framework;
using System;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Hyperplane3DTests
    {
        [Test]
        public void Constructor_FromCoefficients()
        {
            var sut = new Hyperplane3D(1, 2, 3, 4);
            Assert.AreEqual(new Rational(1, 3), sut.A);
            Assert.AreEqual(new Rational(2, 3), sut.B);
            Assert.AreEqual(new Rational(3, 3), sut.C);
            Assert.AreEqual(new Rational(4, 3), sut.D);
        }

        [Test]
        public void Constructor_FromPoints()
        {
            var sut = new Hyperplane3D(
                new Point3D(1, 1, 4),
                new Point3D(-2, -2, 4),
                new Point3D(1, -3, 4)
            );
            Assert.AreEqual((Rational)0, sut.A);
            Assert.AreEqual((Rational)0, sut.B);
            Assert.AreEqual((Rational)1, sut.C);
            Assert.AreEqual(-(Rational)4, sut.D);
        }

        [Test]
        public void Coplane()
        {
            var sut = new Hyperplane3D(1, 2, 3, 4).Coplane;
            Assert.AreEqual(new Rational(-1, 3), sut.A);
            Assert.AreEqual(new Rational(-2, 3), sut.B);
            Assert.AreEqual(new Rational(-3, 3), sut.C);
            Assert.AreEqual(new Rational(-4, 3), sut.D);
        }

        [Test]
        public void Determinants()
        {
            var sut = new Hyperplane3D(1, 2, 3, 4);
            Assert.AreEqual(new Rational(14), sut.Determinant(new Point3D(5, 6, 7)));
            Assert.AreEqual(new Rational(0), sut.Determinant(new Point3D(5, 6, -7)));
            Assert.AreEqual(new Rational(-8), sut.Determinant(new Point3D(5, -6, -7)));

            Assert.AreEqual(1, sut.DetermineHalfspaceOf(new Point3D(5, 6, 7)));
            Assert.AreEqual(0, sut.DetermineHalfspaceOf(new Point3D(5, 6, -7)));
            Assert.AreEqual(-1, sut.DetermineHalfspaceOf(new Point3D(5, -6, -7)));
        }

        [Test]
        public void EqualityAndHashCode()
        {
            TestUtils.TestEqualityAndHashCode(
                () => new Hyperplane3D(1, 0, 0, 0),
                () => new Hyperplane3D(0, 1, 0, 0),
                () => new Hyperplane3D(0, 0, 1, 0),
                () => new Hyperplane3D(0, 0, 1, 1)
            );
        }

        [Test]
        public void Intersect()
        {
            var points = new[]
            {
                new Point3D(1, 0, -4),
                new Point3D(2, -3, -1),
                new Point3D(3, 5, -2),
                new Point3D(6, -6, 4),
            };

            foreach (var P in TestUtils.PermuteIndices(4))
            {
                var planes = new[] {
                    new Hyperplane3D(points[P[0]], points[P[1]], points[P[2]]),
                    new Hyperplane3D(points[P[0]], points[P[2]], points[P[3]]),
                    new Hyperplane3D(points[P[0]], points[P[3]], points[P[1]]),
                };

                foreach (var Q in TestUtils.PermuteIndices(3))
                {
                    Assert.AreEqual(points[P[0]],
                        Hyperplane3D.Intersect(planes[Q[0]], planes[Q[1]], planes[Q[2]]));
                    Assert.IsNull(Hyperplane3D.Intersect(
                        planes[Q[0]], planes[Q[1]], planes[Q[0]]));
                }
            }
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            var point = new Point3D(0, 1, 2);
            var sut = new Hyperplane3D(1, 0, 0, 0);
            TestUtils.NullChecks(new()
            {
                { typeof(ArgumentNullException), new TestDelegate[] {
                    () => { _ = new Hyperplane3D(null, 0, 0, 0 ); },
                    () => { _ = new Hyperplane3D(0, null, 0, 0 ); },
                    () => { _ = new Hyperplane3D(0, 0, null, 0 ); },
                    () => { _ = new Hyperplane3D(0, 0, 0, null ); },
                    () => { _ = new Hyperplane3D(null, point, point); },
                    () => { _ = new Hyperplane3D(point, null, point); },
                    () => { _ = new Hyperplane3D(point, point, null); },
                    () => { sut.DetermineHalfspaceOf(null); },
                    () => { sut.Determinant(null); },
                    () => { Hyperplane3D.Intersect(null, sut, sut); },
                    () => { Hyperplane3D.Intersect(sut, null, sut); },
                    () => { Hyperplane3D.Intersect(sut, sut, null); },

                }},
                { typeof(ArgumentException), new TestDelegate[] {
                    () => { _ = new Hyperplane3D(0, 0, 0, 10); },
                }},
                { typeof(InvalidOperationException), new TestDelegate[]
                {
                    () => { _ = new Hyperplane3D(
                        new Point3D(1, 2, 3), new Point3D(2, 4, 6), new Point3D(3, 6, 9));
                    },
                }}
            });
        }
    }
}
