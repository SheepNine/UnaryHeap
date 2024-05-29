using System;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a rational sphere in 3D space.
    /// </summary>
    public class Sphere3D
    {
        /// <summary>
        /// The cneter of the Sphere3D.
        /// </summary>
        public Point3D Center { get; private set; }

        /// <summary>
        /// The square of the radius of the Sphere3D.
        /// </summary>
        public Rational Quadrance { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Sphere3D class.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="quadrance">The square of the radius of the Sphere3D.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Sphere3D(Point3D center, Rational quadrance)
        {
            if (null == center)
                throw new ArgumentNullException(nameof(center));
            if (null == quadrance)
                throw new ArgumentNullException(nameof(quadrance));

            Center = center;
            Quadrance = quadrance;
        }

        /// <summary>
        /// Initializes a new instance of the Sphere3D class to be the sphere with the given
        /// points on its surface and whose center lies on the plane defined by the given points.
        /// </summary>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <param name="p3">The third point.</param>
        public static Sphere3D Circumcircle(Point3D p1, Point3D p2, Point3D p3)
        {
            if (null == p1)
                throw new ArgumentNullException(nameof(p1));
            if (null == p2)
                throw new ArgumentNullException(nameof(p2));
            if (null == p3)
                throw new ArgumentNullException(nameof(p3));

            Hyperplane3D pointPlane;
            try
            {
                pointPlane = new Hyperplane3D(p1, p2, p3);
            }
            catch (InvalidOperationException)
            {
                // Points not linearly independent
                return null;
            }

            var circumcenter = Hyperplane3D.Intersect(
                pointPlane,
                SplittingPlane(p1, p2),
                SplittingPlane(p2, p3)
            );

            return new Sphere3D(circumcenter, Point3D.Quadrance(p1, circumcenter));
        }

        static Hyperplane3D SplittingPlane(Point3D a, Point3D b)
        {
            var vector = new Point3D(b.X - a.X, b.Y - a.Y, b.Z - a.Z);
            var center = new Point3D((a.X + b.X) / 2, (a.Y + b.Y) / 2, (a.Z + b.Z) / 2);

            return new Hyperplane3D(vector.X, vector.Y, vector.Z,
                -(vector.X * center.X + vector.Y * center.Y + vector.Z * center.Z));
        }

        /// <summary>
        /// Classify a point against this Sphere3D.
        /// </summary>
        /// <param name="point">The point to classify.</param>
        /// <returns>A positive value, if the point is outside the sphere.
        /// Zero, if the point lies on the surface of the sphere.
        /// A negative value, if the point is inside the sphere.</returns>
        public int DetermineHalfspaceOf(Point3D point)
        {
            if (null == point)
                throw new ArgumentNullException(nameof(point));

            return Determinant(point).Sign;
        }

        Rational Determinant(Point3D point)
        {
            return Point3D.Quadrance(point, Center) - Quadrance;
        }
    }
}
