using System;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a hyperplane in two-dimensional space (i.e. a line).
    /// </summary>
    public class Hyperplane2D : IEquatable<Hyperplane2D>
    {
        /// <summary>
        /// The line's X coefficient.
        /// </summary>
        public Rational A { get; private set; }
        /// <summary>
        /// The line's Y coefficient.
        /// </summary>
        public Rational B { get; private set; }
        /// <summary>
        /// The line's constant term.
        /// </summary>
        public Rational C { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Hyperplane2D class.
        /// </summary>
        /// <param name="a">The X coefficient of the line.</param>
        /// <param name="b">The Y coefficient of the line.</param>
        /// <param name="c">The constant term of the line.</param>
        /// <exception cref="System.ArgumentNullException">a, b, or c are null.</exception>
        /// <exception cref="System.ArgumentException">a and b are both equal to zero.</exception>
        public Hyperplane2D(Rational a, Rational b, Rational c)
        {
            if (null == a)
                throw new ArgumentNullException(nameof(a));
            if (null == b)
                throw new ArgumentNullException(nameof(b));
            if (null == c)
                throw new ArgumentNullException(nameof(c));
            if (0 == a && 0 == b)
                throw new ArgumentException("Hyperplane normal has zero length.");

            A = a;
            B = b;
            C = c;

            NormalizeCoefficients();
        }

        /// <summary>
        /// Initializes a new instance of the Hyperplane2D class from two points.
        /// </summary>
        /// <param name="p1">The first point of the line.</param>
        /// <param name="p2">The second point of the line.</param>
        /// <remarks>If p1 is (0,0) and p2 is (1, 0), then the 'front' halfspace
        /// of the resulting Hyperplane2D will be the +Y space. This is by analogy to
        /// the 3D case, where (0,0,0), (1,0,0), (0,1,0) would yield +Z as the front
        /// halfspace.</remarks>
        /// <exception cref="System.ArgumentNullException">p1 or p2 are null.</exception>
        /// <exception cref="System.ArgumentException">p1 equals p2.</exception>
        public Hyperplane2D(Point2D p1, Point2D p2)
        {
            if (null == p1)
                throw new ArgumentNullException(nameof(p1));
            if (null == p2)
                throw new ArgumentNullException(nameof(p2));
            if (p1.Equals(p2))
                throw new ArgumentException("Input points are identical.");

            A = p1.Y - p2.Y;
            B = p2.X - p1.X;
            C = -(A * p1.X + B * p1.Y);

            NormalizeCoefficients();
        }

        void NormalizeCoefficients()
        {
            // --- 'Normalize' the coefficients to optimize equality testing
            // --- to simply be direct comparison of values.

            var d = Rational.Max(A.AbsoluteValue, B.AbsoluteValue);
            A /= d;
            B /= d;
            C /= d;
        }

        /// <summary>
        /// Gets a copy of the current Hyperplane2D, with the front and half
        /// halfspaces swapped.
        /// </summary>
        public Hyperplane2D Coplane
        {
            get { return new Hyperplane2D(-A, -B, -C); }
        }

        /// <summary>
        /// Determines where the given point lies in relation to this Hyperplane2D.
        /// </summary>
        /// <param name="p">The point to classify.</param>
        /// <returns>A positive value, if p is in the front halfspace of this Hyperplane2D.
        /// A negative value, if p is in the back halfspace of this Hyperplane2D.
        /// Zero, if p is incident to this Hyperplane2D.</returns>
        /// <exception cref="System.ArgumentNullException">p is null.</exception>
        public int DetermineHalfspaceOf(Point2D p)
        {
            if (null == p)
                throw new ArgumentNullException(nameof(p));

            return (A * p.X + B * p.Y + C).Sign;
        }

        /// <summary>
        /// Determines the point at which two Hyperplane2Ds intersect.
        /// </summary>
        /// <param name="other">The plane to intersect with this plane.</param>
        /// <returns>The point of intersection, or null if there is not a unique
        /// intersection point (Hyperplanes are coincident or parallel).</returns>
        /// <exception cref="System.ArgumentNullException">other is null.</exception>
        public Point2D FindIntersection(Hyperplane2D other)
        {
            if (null == other)
                throw new ArgumentNullException(nameof(other));

            // Solve the following matrix equation (this is 1, other is 2):
            // |A1 B2| |X|   |-C1|
            // |A2 B2| |Y| = |-C2|

            var determinant = this.A * other.B - other.A * this.B;

            if (0 == determinant)
                return null;

            var x = this.B * other.C - other.B * this.C;
            var y = this.C * other.A - other.C * this.A;

            return new Point2D(x / determinant, y / determinant);
        }

        /// <summary>
        /// Computes the squared distance of a Point2D and the current Hyperplane2D
        /// instance.
        /// </summary>
        /// <param name="p">The point for which to compute the quadrance.</param>
        /// <returns>The squared distance between p and the closest point to p on
        /// the current Hyperplane2D instance.</returns>
        public Rational Quadrance(Point2D p)
        {
            if (null == p)
                throw new ArgumentNullException(nameof(p));

            var determinant = A * p.X + B * p.Y + C;
            var normalQuadrance = A.Squared + B.Squared;

            // TODO: Make ClosestPointTo(Point2D p)
            var closestPoint = new Point2D(
                p.X - (determinant * A) / normalQuadrance,
                p.Y - (determinant * B) / normalQuadrance);

            return Point2D.Quadrance(p, closestPoint);
        }

        /// <summary>
        /// Indicates whether the current Hyperplane2D is equal to another Hyperplane2D.
        /// </summary>
        /// <param name="other">The Hyperplane to compare with this Hyperplane.</param>
        /// <returns>true if the current Hyperplane is equal to the other parameter;
        /// otherwise, false.</returns>
        public bool Equals(Hyperplane2D other)
        {
            if (null == other)
                return false;

            return
                this.A == other.A &&
                this.B == other.B &&
                this.C == other.C;
        }


        /// <summary>
        /// Indicates whether the current Hyperplane2D is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare with this Hyperplane.</param>
        /// <returns>true if the current Hyperplane is equal to the obj parameter;
        /// otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Hyperplane2D);
        }

        /// <summary>
        /// Serves as a hash function for the Hyperplane2D type.
        /// </summary>
        /// <returns>A hash code for the current Hyperplane2D object.</returns>
        public override int GetHashCode()
        {
            return new Point3D(A, B, C).GetHashCode();
        }
    }
}
