using System;
using System.Collections.Generic;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a two-dimensional closed interval; ie, a rectangular prism.
    /// </summary>
    public class Orthotope3D
    {
        Range x;
        Range y;
        Range z;

        /// <summary>
        /// Intializes a new instance of the Orthotope3D class.
        /// </summary>
        /// <param name="x">The x-interval of the new instance.</param>
        /// <param name="y">The y-interval of the new instance.</param>
        /// <param name="z">The z-interval of the new instance.</param>
        /// <exception cref="System.ArgumentNullException">x, y or z are null.</exception>
        public Orthotope3D(Range x, Range y, Range z)
        {
            if (null == x)
                throw new ArgumentNullException(nameof(x));
            if (null == y)
                throw new ArgumentNullException(nameof(y));
            if (null == z)
                throw new ArgumentNullException(nameof(z));

            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Intializes a new instance of the Orthotope3D class.
        /// </summary>
        /// <param name="minX">The smallest value in the x interval.</param>
        /// <param name="minY">The smallest value in the y interval.</param>
        /// <param name="minZ">The smallest value in the z interval.</param>
        /// <param name="maxX">The largest value in the x interval.</param>
        /// <param name="maxY">The largest value in the y interval.</param>
        /// <param name="maxZ">The largest value in the z interval.</param>
        /// <exception cref="System.ArgumentNullException">
        /// minX, minY, minZ, maxX maxY or maxZ are null.</exception>
        /// <exception cref="System.ArgumentException">
        /// minX is greater than maxX, minY is greater than maxY, or
        /// minZ is greater than maxZ.</exception>
        public Orthotope3D(Rational minX, Rational minY, Rational minZ,
            Rational maxX, Rational maxY, Rational maxZ)
            : this(new Range(minX, maxX), new Range(minY, maxY), new Range(minZ, maxZ))
        {
        }

        /// <summary>
        /// Initializes a new instance of the Orthotope3D class as the bounding box
        /// of a set of points.
        /// </summary>
        /// <param name="points">The points from which to compute the bounding box.</param>
        /// <returns>A new Orthotope3D object with the smallest range containing
        /// all of the input points.</returns>
        /// <exception cref="System.ArgumentNullException">points is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// points is empty, or contains a null value.</exception>
        public static Orthotope3D FromPoints(IEnumerable<Point3D> points)
        {
            if (null == points)
                throw new ArgumentNullException(nameof(points));

            Rational minX = null;
            Rational minY = null;
            Rational minZ = null;
            Rational maxX = null;
            Rational maxY = null;
            Rational maxZ = null;
            bool initialized = false;

            foreach (var value in points)
            {
                if (null == value)
                    throw new ArgumentNullException(nameof(points));

                if (null == minX)
                {
                    minX = value.X;
                    minY = value.Y;
                    minZ = value.Z;
                    maxX = value.X;
                    maxY = value.Y;
                    maxZ = value.Z;
                    initialized = true;
                }
                else
                {
                    minX = Rational.Min(minX, value.X);
                    minY = Rational.Min(minY, value.Y);
                    minZ = Rational.Min(minZ, value.Z);
                    maxX = Rational.Max(maxX, value.X);
                    maxY = Rational.Max(maxY, value.Y);
                    maxZ = Rational.Max(maxZ, value.Z);
                }
            }

            if (false == initialized)
                throw new ArgumentException("Enumerable is empty.", nameof(points));

            return new Orthotope3D(minX, minY, minZ, maxX, maxY, maxZ);
        }

        /// <summary>
        /// Gets the x-axis interval.
        /// </summary>
        public Range X
        {
            get { return x; }
        }

        /// <summary>
        /// Gets the y-axis interval.
        /// </summary>
        public Range Y
        {
            get { return y; }
        }

        /// <summary>
        /// Gets the z-axis interval.
        /// </summary>
        public Range Z
        {
            get { return z; }
        }

        /// <summary>
        /// Gets the Point3D of the center of the current Orthotope2D.
        /// </summary>
        public Point3D Center
        {
            get { return new Point3D(X.Midpoint, Y.Midpoint, Z.Midpoint); }
        }

        /// <summary>
        /// Determines whether the specified Point3D value lies within the
        /// current Orthotope3D.
        /// </summary>
        /// <param name="value">The value to check for membership.</param>
        /// <returns>True, if value.X lies within the X range, value.Y
        /// lies within the Y range and value.Z lies within the Z range.</returns>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        public bool Contains(Point3D value)
        {
            if (null == value)
                throw new ArgumentNullException(nameof(value));

            return x.Contains(value.X) && y.Contains(value.Y) && z.Contains(value.Z);
        }

        /// <summary>
        /// Gets a new Orthotope3D object whose endpoints wre offset from the
        /// current Orthotope3D instance.
        /// </summary>
        /// <param name="thickness">The amount by which to offset.</param>
        /// <returns>A new Orthotope3D object with each axis padded by
        /// the specified amount.</returns>
        /// <exception cref="System.ArgumentNullException">thickness is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// thickness is negative and more than half of X.Size, Y.Size or Z.Size.</exception>
        public Orthotope3D GetPadded(Rational thickness)
        {
            return new Orthotope3D(x.GetPadded(thickness), y.GetPadded(thickness),
                z.GetPadded(thickness));
        }

        /// <summary>
        /// Gets a new Orthotope3D object whose size is a multiple of the
        /// current Orthotope3D instance.
        /// </summary>
        /// <param name="factor">The amount by which to scale.</param>
        /// <returns>A new Orthotope3D object with each axis scaled by
        /// the specified amount.</returns>
        /// <exception cref="System.ArgumentNullException">factor is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// factor is negative.</exception>
        public Orthotope3D GetScaled(Rational factor)
        {
            return new Orthotope3D(x.GetScaled(factor), y.GetScaled(factor),
                z.GetScaled(factor));
        }

        /// <summary>
        /// Gets a new Orthotope3D object with the same area whose center
        /// is equal to the center specified.
        /// </summary>
        /// <param name="center">The center of the new Orthotope3D.</param>
        /// <returns>A new Orthotope3D object with the same area whose center
        /// is equal to the center specified.</returns>
        /// <exception cref="System.ArgumentNullException">center is null.
        /// </exception>
        public Orthotope3D CenteredAt(Point3D center)
        {
            if (null == center)
                throw new ArgumentNullException(nameof(center));

            return new Orthotope3D(x.CenteredAt(center.X), y.CenteredAt(center.Y),
                z.CenteredAt(center.Z));
        }

        /// <summary>
        /// Constructs a set of facets corresponding to the inside of this instance.
        /// </summary>
        /// <returns>
        /// Facets for each side of this instance, with surface normals facing inwards.
        /// </returns>
        public IEnumerable<Facet3D> MakeFacets()
        {
            var points = new[]
            {
                new Point3D(X.Min, Y.Min, Z.Min),
                new Point3D(X.Min, Y.Max, Z.Min),
                new Point3D(X.Min, Y.Min, Z.Max),
                new Point3D(X.Min, Y.Max, Z.Max),
                new Point3D(X.Max, Y.Min, Z.Max),
                new Point3D(X.Max, Y.Max, Z.Max),
                new Point3D(X.Max, Y.Min, Z.Min),
                new Point3D(X.Max, Y.Max, Z.Min),
            };

            return new[]
            {
                new Facet3D(new Hyperplane3D(1, 0, 0, -X.Min),
                    new[] { points[0], points[1], points[3], points[2] }),
                new Facet3D(new Hyperplane3D(0, 0, -1, Z.Max),
                    new[] { points[2], points[3], points[5], points[4] }),
                new Facet3D(new Hyperplane3D(-1, 0, 0, X.Max),
                    new[] { points[4], points[5], points[7], points[6] }),
                new Facet3D(new Hyperplane3D(0, 0, 1, -Z.Min),
                    new[] { points[6], points[7], points[1], points[0] }),
                new Facet3D(new Hyperplane3D(0, 1, 0, -Y.Min),
                    new[] { points[0], points[2], points[4], points[6] }),
                new Facet3D(new Hyperplane3D(0, -1, 0, Y.Max),
                    new[] { points[7], points[5], points[3], points[1] })
            };
        }

        /// <summary>
        /// Checks whether this Orthotope3D and another Orthotope3D share any points.
        /// </summary>
        /// <param name="other">The Orthotope3D to check against.</param>
        /// <returns>true if at least one Point3D is contained in both instances. </returns>
        public bool Intersects(Orthotope3D other)
        {
            throw new NotImplementedException();
        }
    }
}
