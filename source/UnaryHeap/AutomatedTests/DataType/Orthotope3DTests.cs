using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Orthotope3DTests
    {
        [Test]
        public void Constructor_Values()
        {
            var sut = new Orthotope3D(1, 2, 3, 4, 5, 6);
            Assert.AreEqual((Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)2, sut.Y.Min);
            Assert.AreEqual((Rational)3, sut.Z.Min);
            Assert.AreEqual((Rational)4, sut.X.Max);
            Assert.AreEqual((Rational)5, sut.Y.Max);
            Assert.AreEqual((Rational)6, sut.Z.Max);
        }

        [Test]
        public void Constructor_Ranges()
        {
            var sut = new Orthotope3D(new Range(1, 2), new Range(3, 4), new Range(5, 6));
            Assert.AreEqual((Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)2, sut.X.Max);
            Assert.AreEqual((Rational)3, sut.Y.Min);
            Assert.AreEqual((Rational)4, sut.Y.Max);
            Assert.AreEqual((Rational)5, sut.Z.Min);
            Assert.AreEqual((Rational)6, sut.Z.Max);
        }

        [Test]
        public void FromPoints()
        {
            var sut = Orthotope3D.FromPoints(new[] {
                new Point3D(1, 5, 3), new Point3D(4, 2, 6) });
            Assert.AreEqual((Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)2, sut.Y.Min);
            Assert.AreEqual((Rational)3, sut.Z.Min);
            Assert.AreEqual((Rational)4, sut.X.Max);
            Assert.AreEqual((Rational)5, sut.Y.Max);
            Assert.AreEqual((Rational)6, sut.Z.Max);
        }

        [Test]
        public void FromOnePoint()
        {
            var sut = Orthotope3D.FromPoints(new[] { new Point3D(1, 2, 3) });
            Assert.AreEqual((Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)2, sut.Y.Min);
            Assert.AreEqual((Rational)3, sut.Z.Min);
            Assert.AreEqual((Rational)1, sut.X.Max);
            Assert.AreEqual((Rational)2, sut.Y.Max);
            Assert.AreEqual((Rational)3, sut.Z.Max);
        }

        [Test]
        public void Contains()
        {
            var sut = new Orthotope3D(1, 2, 3, 4, 5, 6);
            Assert.IsTrue(sut.Contains(new Point3D(2, 4, 5)));
            Assert.IsFalse(sut.Contains(new Point3D(0, 4, 5)));
            Assert.IsFalse(sut.Contains(new Point3D(5, 4, 5)));
            Assert.IsFalse(sut.Contains(new Point3D(2, 1, 5)));
            Assert.IsFalse(sut.Contains(new Point3D(2, 6, 5)));
            Assert.IsFalse(sut.Contains(new Point3D(2, 4, 2)));
            Assert.IsFalse(sut.Contains(new Point3D(2, 4, 7)));
        }

        [Test]
        public void GetPadded()
        {
            var sut = new Orthotope3D(1, 2, 3, 4, 5, 6).GetPadded(4);
            Assert.AreEqual(-(Rational)3, sut.X.Min);
            Assert.AreEqual(-(Rational)2, sut.Y.Min);
            Assert.AreEqual(-(Rational)1, sut.Z.Min);
            Assert.AreEqual((Rational)8, sut.X.Max);
            Assert.AreEqual((Rational)9, sut.Y.Max);
            Assert.AreEqual((Rational)10, sut.Z.Max);
        }

        [Test]
        public void GetScaled()
        {
            var sut = new Orthotope3D(1, 2, 3, 5, 6, 7).GetScaled(2);
            Assert.AreEqual(-(Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)0, sut.Y.Min);
            Assert.AreEqual((Rational)1, sut.Z.Min);
            Assert.AreEqual((Rational)7, sut.X.Max);
            Assert.AreEqual((Rational)8, sut.Y.Max);
            Assert.AreEqual((Rational)9, sut.Z.Max);
        }

        [Test]
        public void CenteredAt()
        {
            var sut = new Orthotope3D(1, 2, 3, 5, 6, 7).CenteredAt(new Point3D(7, 8, 9));
            Assert.AreEqual((Rational)5, sut.X.Min);
            Assert.AreEqual((Rational)6, sut.Y.Min);
            Assert.AreEqual((Rational)7, sut.Z.Min);
            Assert.AreEqual((Rational)9, sut.X.Max);
            Assert.AreEqual((Rational)10, sut.Y.Max);
            Assert.AreEqual((Rational)11, sut.Z.Max);
        }

        [Test]
        public void Intersects()
        {
            var sut = new Orthotope3D(2, 3, 4, 5, 6, 7);
            Assert.IsTrue(sut.Intersects(new Orthotope3D(0, 1, 2, 7, 8, 9)));
            Assert.IsFalse(sut.Intersects(new Orthotope3D(0, 1, 2, 1, 8, 9)));
            Assert.IsFalse(sut.Intersects(new Orthotope3D(6, 1, 2, 7, 8, 9)));
            Assert.IsFalse(sut.Intersects(new Orthotope3D(0, 1, 2, 7, 2, 9)));
            Assert.IsFalse(sut.Intersects(new Orthotope3D(0, 7, 2, 7, 8, 9)));
            Assert.IsFalse(sut.Intersects(new Orthotope3D(0, 1, 2, 7, 8, 3)));
            Assert.IsFalse(sut.Intersects(new Orthotope3D(0, 1, 8, 7, 8, 9)));
        }

        [Test]
        public void Facetize3D()
        {
            foreach (var sut in new[]
            {
                new Orthotope3D(-1, -1, -1, 0, 0, 0),
                new Orthotope3D(0, 0, 0, 1, 1, 1),
                new Orthotope3D(-1, -1, -1, 1, 1, 1),
                new Orthotope3D(-3, -2, 1, -1, 0, 6)
            })
                TestMakeFacets(sut);
        }

        static void TestMakeFacets(Orthotope3D sut)
        {
            var facets = sut.MakeFacets().ToList();

            // There are six different planes
            Assert.AreEqual(6, facets.Select(facet => facet.Plane).Distinct().Count());

            foreach (var facet in facets)
            {
                // Facet points lie on its plane
                foreach (var point in facet.Points)
                    Assert.AreEqual(0, facet.Plane.DetermineHalfspaceOf(point));
                // Facet plane is derived from its points
                Assert.AreEqual(facet.Plane, new Hyperplane3D(facet.Points.ElementAt(0),
                    facet.Points.ElementAt(1), facet.Points.ElementAt(2)));
                Assert.AreEqual(facet.Plane, new Hyperplane3D(facet.Points.ElementAt(1),
                    facet.Points.ElementAt(2), facet.Points.ElementAt(3)));
                Assert.AreEqual(facet.Plane, new Hyperplane3D(facet.Points.ElementAt(2),
                    facet.Points.ElementAt(3), facet.Points.ElementAt(0)));
                Assert.AreEqual(facet.Plane, new Hyperplane3D(facet.Points.ElementAt(3),
                    facet.Points.ElementAt(0), facet.Points.ElementAt(1)));
                // Facet faces towards the orhtotope center
                Assert.AreEqual(1, facet.Plane.DetermineHalfspaceOf(sut.Center));
            }

            foreach (var corner in new[]
            {
                new Point3D(sut.X.Min, sut.Y.Min, sut.Z.Min),
                new Point3D(sut.X.Max, sut.Y.Min, sut.Z.Min),
                new Point3D(sut.X.Min, sut.Y.Max, sut.Z.Min),
                new Point3D(sut.X.Max, sut.Y.Max, sut.Z.Min),
                new Point3D(sut.X.Min, sut.Y.Min, sut.Z.Max),
                new Point3D(sut.X.Max, sut.Y.Min, sut.Z.Max),
                new Point3D(sut.X.Min, sut.Y.Max, sut.Z.Max),
                new Point3D(sut.X.Max, sut.Y.Max, sut.Z.Max),
            })
            {
                // The corner lies on three faces
                Assert.AreEqual(3, facets.Count(
                    facet => facet.Plane.DetermineHalfspaceOf(corner) == 0));
                // The corner is in front of the other three faces
                Assert.AreEqual(3, facets.Count(
                    facet => facet.Plane.DetermineHalfspaceOf(corner) == 1));
            }
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            var validRange = new Range(1, 2);
            var validSut = new Orthotope3D(0, 0, 0, 1, 1, 1);

            NullChecks(new()
            {
                { typeof(ArgumentNullException), new TestDelegate[] {
                    () => { _ = new Orthotope3D(null, validRange, validRange); },
                    () => { _ = new Orthotope3D(validRange, null, validRange); },
                    () => { _ = new Orthotope3D(validRange, validRange, null); },
                    () => { Orthotope3D.FromPoints(null); },
                    () => { Orthotope3D.FromPoints(new Point3D[] { null } ); },
                    () => { validSut.Contains(null); },
                    () => { validSut.GetPadded(null); },
                    () => { validSut.GetScaled(null); },
                    () => { validSut.CenteredAt(null); },
                    () => { validSut.Intersects(null); },
                } },
                { typeof(ArgumentException), new TestDelegate[]
                {
                    () => { Orthotope3D.FromPoints(Array.Empty<Point3D>()); },
                } }
            });
        }

        static void NullChecks(Dictionary<Type, IEnumerable<TestDelegate>> testCases)
        {
            foreach (var testCase in testCases)
                foreach (var action in testCase.Value)
                    Assert.Throws(testCase.Key, action);
        }
    }
}
