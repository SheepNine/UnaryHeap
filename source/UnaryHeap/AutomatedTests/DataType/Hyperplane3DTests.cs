using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnaryHeap.DataType;

namespace UnaryHeap.DataType.Tests
{
    /// <summary>
    /// Represents a hyperplane in two-dimensional space (i.e. a plane).
    /// </summary>
    class Hyperplane3D
    {
        /// <summary>
        /// The plane normal's X coefficient.
        /// </summary>
        public Rational A { get; private set; }
        /// <summary>
        /// The plane normal's Y coefficient.
        /// </summary>
        public Rational B { get; private set; }
        /// <summary>
        /// The plane normal's Z coefficient.
        /// </summary>
        public Rational C { get; private set; }
        /// <summary>
        /// The plane normal's constant term.
        /// </summary>
        public Rational D { get; private set; }

        public Hyperplane3D(Point3D p1, Point3D p2, Point3D p3)
        {
            if (null == p1)
                throw new ArgumentNullException(nameof(p1));
            if (null == p2)
                throw new ArgumentNullException(nameof(p2));
            if (null == p3)
                throw new ArgumentNullException(nameof(p3));

            var v1X = p2.X - p1.X;
            var v1Y = p2.Y - p1.Y;
            var v1Z = p2.Z - p1.Z;

            var v2X = p3.X - p1.X;
            var v2Y = p3.Y - p1.Y;
            var v2Z = p3.Z - p1.Z;

            var nX = v1Y * v2Z - v2Y * v1Z;
            var nY = v1Z * v2X - v2Z * v1X;
            var nZ = v1X * v2Y - v2X * v1Y;

            if (nX == 0 && nY == 0 && nZ == 0)
                throw new InvalidOperationException("Points are not linearly independent");

            A = nX;
            B = nY;
            C = nZ;
            D = -(A * p1.X + B * p1.Y + C * p1.Z);

            NormalizeCoefficients();
        }

        /// <summary>
        /// Initializes a new instance of the Hyperplane3D class.
        /// </summary>
        /// <param name="a">The X coefficient of the plane normal.</param>
        /// <param name="b">The Y coefficient of the plane normal.</param>
        /// <param name="c">The Z coefficient of the plane normal.</param>
        /// <param name="d">The constant term of the plane normal.</param>
        /// <exception cref="System.ArgumentNullException">a, b, c or d are null.</exception>
        /// <exception cref="System.ArgumentException">
        /// a, b and c are all equal to zero.
        /// </exception>
        public Hyperplane3D(Rational a, Rational b, Rational c, Rational d)
        {
            if (null == a)
                throw new ArgumentNullException(nameof(a));
            if (null == b)
                throw new ArgumentNullException(nameof(b));
            if (null == c)
                throw new ArgumentNullException(nameof(c));
            if (null == d)
                throw new ArgumentNullException(nameof(d));
            if (0 == a && 0 == b && 0 == c)
                throw new ArgumentException("Hyperplane normal has zero length.");

            A = a;
            B = b;
            C = c;
            D = d;

            NormalizeCoefficients();
        }

        /// <summary>
        /// Determines where the given point lies in relation to this Hyperplane3D.
        /// </summary>
        /// <param name="p">The point to classify.</param>
        /// <returns>A positive value, if p is in the front halfspace of this Hyperplane3D.
        /// A negative value, if p is in the back halfspace of this Hyperplane3D.
        /// Zero, if p is incident to this Hyperplane3D.</returns>
        /// <exception cref="System.ArgumentNullException">p is null.</exception>
        public int DetermineHalfspaceOf(Point3D p)
        {
            if (null == p)
                throw new ArgumentNullException(nameof(p));

            return (A * p.X + B * p.Y + C * p.Z + D).Sign;
        }

        void NormalizeCoefficients()
        {
            // --- 'Normalize' the coefficients to optimize equality testing
            // --- to simply be direct comparison of values.

            var e = Rational.Max(A.AbsoluteValue, Rational.Max(B.AbsoluteValue, C.AbsoluteValue));
            A /= e;
            B /= e;
            C /= e;
            D /= e;
        }


        /// <summary>
        /// Gets a copy of the current Hyperplane3D, with the front and half
        /// halfspaces swapped.
        /// </summary>
        public Hyperplane3D Coplane
        {
            get { return new Hyperplane3D(-A, -B, -C, -D); }
        }

        /// <summary>
        /// Indicates whether the current Hyperplane3D is equal to another Hyperplane3D.
        /// </summary>
        /// <param name="other">The Hyperplane to compare with this Hyperplane.</param>
        /// <returns>true if the current Hyperplane is equal to the other parameter;
        /// otherwise, false.</returns>
        public bool Equals(Hyperplane3D other)
        {
            if (null == other)
                return false;

            return
                this.A == other.A &&
                this.B == other.B &&
                this.C == other.C &&
                this.D == other.D;
        }


        /// <summary>
        /// Indicates whether the current Hyperplane3D is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare with this Hyperplane.</param>
        /// <returns>true if the current Hyperplane is equal to the obj parameter;
        /// otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Hyperplane3D);
        }

        /// <summary>
        /// Serves as a hash function for the Hyperplane3D type.
        /// </summary>
        /// <returns>A hash code for the current Hyperplane3D object.</returns>
        public override int GetHashCode()
        {
            return new Point3D(A, B, C).GetHashCode() ^ D.GetHashCode();
        }

        public List<Point3D> MakePolytope(Rational size)
        {
            // TODO: this method will return incorrect windings for some planes;
            // need to reverse the order sometimes
            // Comprehensive unit tests will ferret it out
            if (A != 0)
            {
                var sign = A.Sign;
                return new List<Point3D>()
                {
                    SolveForX(size, size),
                    SolveForX(-size * sign, size * sign),
                    SolveForX(-size, -size),
                    SolveForX(size * sign, -size * sign),
                };
            }
            else if (B != 0)
            {
                var sign = B.Sign;
                return new List<Point3D>()
                {
                    SolveForY(size, size),
                    SolveForY(size * sign, -size * sign),
                    SolveForY(-size, -size),
                    SolveForY(-size * sign, size * sign),
                };
            }
            else // Constructor affirms that C != 0
            {
                var sign = C.Sign;
                return new List<Point3D>()
                {
                    SolveForZ(size, size),
                    SolveForZ(-size * sign, size * sign),
                    SolveForZ(-size, -size),
                    SolveForZ(size * sign, -size * sign),
                };
            }
        }

        Point3D SolveForX(Rational y, Rational z)
        {
            return new Point3D(-(B * y + C * z + D) / A, y, z);
        }

        Point3D SolveForY(Rational x, Rational z)
        {
            return new Point3D(x, -(A * x + C * z + D) / B, z);
        }

        Point3D SolveForZ(Rational x, Rational y)
        {
            return new Point3D(x, y, -(A * x + B * y + D) / C);
        }

        public Point3D Pierce(Point3D p1, Point3D p2)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class Hyperplane3DTests
    {
        private static void CheckPointConstructor(
            Point3D A, Point3D B, Point3D C, Hyperplane3D front)
        {
            Assert.AreEqual(0, front.DetermineHalfspaceOf(A));
            Assert.AreEqual(0, front.DetermineHalfspaceOf(B));
            Assert.AreEqual(0, front.DetermineHalfspaceOf(C));
            Assert.AreEqual(front, new Hyperplane3D(A, B, C));
            Assert.AreEqual(front, new Hyperplane3D(B, C, A));
            Assert.AreEqual(front, new Hyperplane3D(C, A, B));
            Assert.AreEqual(front.Coplane, new Hyperplane3D(A, C, B));
            Assert.AreEqual(front.Coplane, new Hyperplane3D(B, A, C));
            Assert.AreEqual(front.Coplane, new Hyperplane3D(C, B, A));
        }

        [Test]
        public void XPlane()
        {
            var plane = new Hyperplane3D(1, 0, 0, 0);
            CheckPointConstructor(Point3D.Origin, new Point3D(0, 1, 0), new Point3D(0, 0, 1),
                plane);
        }

        [Test]
        public void YPlane()
        {
            var plane = new Hyperplane3D(0, 1, 0, 0);
            CheckPointConstructor(Point3D.Origin, new Point3D(0, 0, 1), new Point3D(1, 0, 0),
                plane);
        }

        [Test]
        public void ZPlane()
        {
            var plane = new Hyperplane3D(0, 0, 1, 0);
            CheckPointConstructor(Point3D.Origin, new Point3D(1, 0, 0), new Point3D(0, 1, 0),
                plane);
        }

        private static void CheckPolytope(Hyperplane3D sut)
        {
            var points = sut.MakePolytope(100);
            foreach (var point in points)
                Assert.AreEqual(0, sut.DetermineHalfspaceOf(point));

            var A = points[0];
            var B = points[1];
            var C = points[2];
            var D = points[3];

            Assert.AreEqual(sut, new Hyperplane3D(A, B, C));
            Assert.AreEqual(sut, new Hyperplane3D(B, C, A));
            Assert.AreEqual(sut, new Hyperplane3D(C, A, B));
            Assert.AreEqual(sut, new Hyperplane3D(A, B, D));
            Assert.AreEqual(sut, new Hyperplane3D(B, D, A));
            Assert.AreEqual(sut, new Hyperplane3D(D, A, B));
            Assert.AreEqual(sut, new Hyperplane3D(A, C, D));
            Assert.AreEqual(sut, new Hyperplane3D(C, D, A));
            Assert.AreEqual(sut, new Hyperplane3D(D, A, C));
            Assert.AreEqual(sut, new Hyperplane3D(B, C, D));
            Assert.AreEqual(sut, new Hyperplane3D(C, D, B));
            Assert.AreEqual(sut, new Hyperplane3D(D, B, C));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(C, B, A));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(B, A, C));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(A, C, B));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(D, B, A));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(B, A, D));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(A, D, B));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(D, C, A));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(C, A, D));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(A, D, C));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(D, C, B));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(C, B, D));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(B, D, C));
        }

        [Test]
        public void XPlaneWinding()
        {
            CheckPolytope(new Hyperplane3D(1, 0, 0, 0));
            CheckPolytope(new Hyperplane3D(-1, 0, 0, 0));
        }

        [Test]
        public void YPlaneWinding()
        {
            CheckPolytope(new Hyperplane3D(0, 1, 0, 0));
            CheckPolytope(new Hyperplane3D(0, -1, 0, 0));
        }

        [Test]
        public void ZPlaneWinding()
        {
            CheckPolytope(new Hyperplane3D(0, 0, 1, 0));
            CheckPolytope(new Hyperplane3D(0, 0, -1, 0));
        }

        [Test]
        public void Winding()
        {
            var points = new Hyperplane3D(1, 1, 1, 0).MakePolytope(10);
            Assert.AreEqual(points[0], new Point3D(-20, 10, 10));
            Assert.AreEqual(points[1], new Point3D(0, -10, 10));
            Assert.AreEqual(points[2], new Point3D(20, -10, -10));
            Assert.AreEqual(points[3], new Point3D(0, 10, -10));
        }
    }
}
