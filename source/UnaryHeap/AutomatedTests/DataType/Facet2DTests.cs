using NUnit.Framework;
using System;

namespace UnaryHeap.DataType.Tests
{
    class Facet2D
    {
        public Point2D Start { get; private set; }
        public Point2D End { get; private set; }
        public Hyperplane2D Plane { get; private set; }

        public Facet2D(Hyperplane2D plane, Point2D start, Point2D end)
        {
            Plane = plane;
            Start = start;
            End = end;
        }

        public Facet2D(Hyperplane2D plane, Rational radius)
        {
            throw new NotImplementedException("TODO");
        }

        public void Split(Hyperplane2D splittingPlane,
            out Facet2D frontFacet, out Facet2D backFacet)
        {
            if (splittingPlane.Equals(Plane))
            {
                frontFacet = this;
                backFacet = null;
                return;
            }
            if (splittingPlane.Coplane.Equals(Plane))
            {
                frontFacet = null;
                backFacet = this;
                return;
            }

            var startDet = splittingPlane.Determinant(Start);
            var endDet = splittingPlane.Determinant(End);

            if (startDet >= 0 && endDet >= 0)
            {
                frontFacet = this;
                backFacet = null;
                return;
            }

            if (startDet <= 0 && endDet <= 0)
            {
                frontFacet = null;
                backFacet = this;
                return;
            }

            var tEnd = startDet / (startDet - endDet);
            var tStart = 1 - tEnd;

            var mid = new Point2D(
                Start.X * tStart + End.X * tEnd,
                Start.Y * tStart + End.Y * tEnd);

            if (startDet >= 0)
            {
                frontFacet = new Facet2D(Plane, Start, mid);
                backFacet = new Facet2D(Plane, mid, End);
            }
            else
            {
                backFacet = new Facet2D(Plane, Start, mid);
                frontFacet = new Facet2D(Plane, mid, End);
            }
        }
    }

    [TestFixture]
    public class Facet2DTests
    {
        [Test]
        public void Split()
        {
            var start = new Point2D(1, 2);
            var end = new Point2D(7, 5);
            var mid = new Point2D(5, 4);

            var sut = new Facet2D(new Hyperplane2D(start, end), start, end);

            // Coplanar
            sut.Split(new Hyperplane2D(start, end), out Facet2D front, out Facet2D back);
            Assert.IsNull(back);
            Assert.AreEqual(start, front.Start);
            Assert.AreEqual(end, front.End);

            // Counterplanar
            sut.Split(new Hyperplane2D(end, start), out front, out back);
            Assert.IsNull(front);
            Assert.AreEqual(start, back.Start);
            Assert.AreEqual(end, back.End);

            // Edge touch
            sut.Split(new Hyperplane2D(start, new Point2D(start.X, start.Y - 1)),
                out front, out back);
            Assert.IsNull(back);
            Assert.AreEqual(start, front.Start);
            Assert.AreEqual(end, front.End);

            sut.Split(new Hyperplane2D(start, new Point2D(start.X, start.Y + 1)),
                out front, out back);
            Assert.IsNull(front);
            Assert.AreEqual(start, back.Start);
            Assert.AreEqual(end, back.End);

            sut.Split(new Hyperplane2D(end, new Point2D(end.X, end.Y + 1)),
                out front, out back);
            Assert.IsNull(back);
            Assert.AreEqual(start, front.Start);
            Assert.AreEqual(end, front.End);

            sut.Split(new Hyperplane2D(end, new Point2D(end.X, end.Y - 1)),
                out front, out back);
            Assert.IsNull(front);
            Assert.AreEqual(start, back.Start);
            Assert.AreEqual(end, back.End);

            // Actual split
            sut.Split(new Hyperplane2D(mid, new Point2D(mid.X, mid.Y + 1)),
                out front, out back);
            Assert.AreEqual(start, front.Start);
            Assert.AreEqual(mid, front.End);
            Assert.AreEqual(mid, back.Start);
            Assert.AreEqual(end, back.End);

            sut.Split(new Hyperplane2D(mid, new Point2D(mid.X, mid.Y - 1)),
                out front, out back);
            Assert.AreEqual(start, back.Start);
            Assert.AreEqual(mid, back.End);
            Assert.AreEqual(mid, front.Start);
            Assert.AreEqual(end, front.End);
        }
    }
}
