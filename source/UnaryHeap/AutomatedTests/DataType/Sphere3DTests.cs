using NUnit.Framework;
using System;

namespace UnaryHeap.DataType.Tests
{
    public class Sphere3DTests
    {
        [Test]
        public void ConstructorAndHalfspace()
        {
            var sut = new Sphere3D(Point3D.Origin, 9);
            Assert.AreEqual(Point3D.Origin, sut.Center);
            Assert.AreEqual((Rational)9, sut.Quadrance);

            Assert.AreEqual(-1, sut.DetermineHalfspaceOf(new Point3D(2, 0, 0)));
            Assert.AreEqual(0, sut.DetermineHalfspaceOf(new Point3D(0, 3, 0)));
            Assert.AreEqual(1, sut.DetermineHalfspaceOf(new Point3D(0, 0, 4)));
        }

        [Test]
        public void Circumcircle()
        {
            var p1 = new Point3D(1 + 2, 2, 3);
            var p2 = new Point3D(1 - 2, 2, 3);
            var p3 = new Point3D(1, 2 + 2, 3);

            var sut = Sphere3D.Circumcircle(p1, p2, p3);

            Assert.AreEqual(new Point3D(1, 2, 3), sut.Center);
            Assert.AreEqual((Rational)4, sut.Quadrance);

            Assert.AreEqual(0, sut.DetermineHalfspaceOf(p1));
            Assert.AreEqual(0, sut.DetermineHalfspaceOf(p2));
            Assert.AreEqual(0, sut.DetermineHalfspaceOf(p3));
        }

        [Test]
        public void Circumcirlce_Colinear()
        {
            Assert.IsNull(Sphere3D.Circumcircle(
                new Point3D(3, 1, -2),
                new Point3D(4, 2, -1),
                new Point3D(7, 5, 2)
            ));
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            var point = new Point3D(1, 2, 3);
            var sphere = new Sphere3D(Point3D.Origin, 1);
            TestUtils.NullChecks(new()
            {
                { typeof(ArgumentNullException), new TestDelegate[] {
                    () => { _ = new Sphere3D(null, 1); },
                    () => { _ = new Sphere3D(Point3D.Origin, null); },
                    () => { Sphere3D.Circumcircle(null, point, point); },
                    () => { Sphere3D.Circumcircle(point, null, point); },
                    () => { Sphere3D.Circumcircle(point, point, null); },
                    () => { sphere.DetermineHalfspaceOf(null); },
                }}
            });
        }
    }
}
