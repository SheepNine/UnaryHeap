using System;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a hyperplane in two-dimensional space (i.e. a plane).
    /// </summary>
    public class Hyperplane3D : IEquatable<Hyperplane3D>
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

        /// <summary>
        /// Initializes a new instance of the Hyperplane3D class. The given points will lie on
        /// the resulting plane, and the plane normal will follow the right-hand rule for the
        /// given points.
        /// </summary>
        /// <param name="p1">A point on the plane.</param>
        /// <param name="p2">Another point on the plane.</param>
        /// <param name="p3">A third point on the plane.</param>
        /// <exception cref="ArgumentNullException">p1, p2 or p3 are null.</exception>
        /// <exception cref="InvalidOperationException">p1, p2 and p3 are colinear.</exception>
        public Hyperplane3D(Point3D p1, Point3D p2, Point3D p3)
        {
            ArgumentNullException.ThrowIfNull(p1);
            ArgumentNullException.ThrowIfNull(p2);
            ArgumentNullException.ThrowIfNull(p3);

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
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            ArgumentNullException.ThrowIfNull(c);
            ArgumentNullException.ThrowIfNull(d);
            if (0 == a && 0 == b && 0 == c)
                throw new ArgumentException("Hyperplane normal has zero length.");

            A = a;
            B = b;
            C = c;
            D = d;

            NormalizeCoefficients();
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
        /// Determines where the given point lies in relation to this Hyperplane3D.
        /// </summary>
        /// <param name="p">The point to classify.</param>
        /// <returns>A positive value, if p is in the front halfspace of this Hyperplane3D.
        /// A negative value, if p is in the back halfspace of this Hyperplane3D.
        /// Zero, if p is incident to this Hyperplane3D.</returns>
        /// <exception cref="System.ArgumentNullException">p is null.</exception>
        public int DetermineHalfspaceOf(Point3D p)
        {
            ArgumentNullException.ThrowIfNull(p);

            return Determinant(p).Sign;
        }

        /// <summary>
        /// Computes the determinant of a point. The magnitude of the result will be positive
        /// for points in the front halfspace of the plane, negative for points in the back
        /// halfspace of the plane, and zero for points on the plane.
        /// Note that as most Hyperplanes do not have unit vectors, the returned value is not
        /// guaranteed to be the exact distance between the plane and the point.
        /// </summary>
        /// <param name="p">The point for which to calculate the determinant.</param>
        /// <returns>The determinant of the point with respect to this hyperplane.</returns>
        /// <exception cref="ArgumentNullException">p is null.</exception>
        public Rational Determinant(Point3D p)
        {
            ArgumentNullException.ThrowIfNull(p);

            return A * p.X + B * p.Y + C * p.Z + D;
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

        /// <summary>
        /// Finds the single point at the intersection of three Hyperplane3Ds.
        /// </summary>
        /// <param name="p1">A plane.</param>
        /// <param name="p2">Another plane.</param>
        /// <param name="p3">Yet another plane.</param>
        /// <returns>The single point at the intersection of three Hyperplane3Ds, or null if the 
        /// planes are not linearly independent.</returns>
        public static Point3D Intersect(Hyperplane3D p1, Hyperplane3D p2, Hyperplane3D p3)
        {
            ArgumentNullException.ThrowIfNull(p1);
            ArgumentNullException.ThrowIfNull(p2);
            ArgumentNullException.ThrowIfNull(p3);

            try
            {
                return new Matrix3D(
                    p1.A, p1.B, p1.C,
                    p2.A, p2.B, p2.C,
                    p3.A, p3.B, p3.C
                ).ComputeInverse()
                    * new Point3D(-p1.D, -p2.D, -p3.D);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}
