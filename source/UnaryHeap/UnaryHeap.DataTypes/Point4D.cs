using System;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a point in four-dimensional space.
    /// </summary>
    public class Point4D : IEquatable<Point4D>
    {
        #region Member Variables

        readonly Rational x;
        readonly Rational y;
        readonly Rational z;
        readonly Rational w;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Point4D class.
        /// </summary>
        /// <param name="x">The x-coordinate of the new point.</param>
        /// <param name="y">The y-coordinate of the new point.</param>
        /// <param name="z">The z-coordinate of the new point.</param>
        /// <param name="w">The w-coordinate of the new point.</param>
        /// <exception cref="System.ArgumentNullException">
        /// x, y, z or w are null references.</exception>
        public Point4D(Rational x, Rational y, Rational z, Rational w)
        {
            if (null == x)
                throw new ArgumentNullException(nameof(x));
            if (null == y)
                throw new ArgumentNullException(nameof(y));
            if (null == z)
                throw new ArgumentNullException(nameof(z));
            if (null == w)
                throw new ArgumentNullException(nameof(w));

            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// Gets a value that represents the point (0, 0, 0, 0).
        /// </summary>
        public static Point4D Origin
        {
            get { return new Point4D(0, 0, 0, 0); }
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the x-cooridante of the current Point4D object.
        /// </summary>
        public Rational X
        {
            get { return x; }
        }

        /// <summary>
        /// Gets the y-cooridante of the current Point4D object.
        /// </summary>
        public Rational Y
        {
            get { return y; }
        }

        /// <summary>
        /// Gets the z-cooridante of the current Point4D object.
        /// </summary>
        public Rational Z
        {
            get { return z; }
        }

        /// <summary>
        /// Gets the w-cooridante of the current Point4D object.
        /// </summary>
        public Rational W
        {
            get { return w; }
        }

        #endregion


        #region Equality

        /// <summary>
        /// Determines whether the specified System.Object object is equal to
        /// the current Point4D object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the obj parameter is a Point4D object or a type capable of implicit
        /// conversion to a Point4D value, and its value is equal to the value of the current
        /// Point4D object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Point4D);
        }

        /// <summary>
        /// Determines whether the specified Point3D object is equal to the
        /// current Point3D object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>true if the value of the obj parameter is equal to the value of
        /// the current Point3D object; otherwise, false.</returns>
        public bool Equals(Point4D other)
        {
            if (null == other)
                return false;

            return this.x == other.x
                && this.y == other.y
                && this.z == other.z
                && this.w == other.w;
        }

        /// <summary>
        /// Serves as a hash function for the Point4D type.
        /// </summary>
        /// <returns>A hash code for the current Point4D object.</returns>
        public override int GetHashCode()
        {
            return (int)(uint)
                (((x.Numerator & 0xF) << 28) |
                 ((x.Denominator & 0xF) << 24) |
                 ((y.Numerator & 0xF) << 20) |
                 ((y.Denominator & 0xF) << 16) |
                 ((z.Numerator & 0xF) << 12) |
                 ((z.Denominator & 0xF) << 08) |
                 ((w.Numerator & 0xF) << 04) |
                 ((w.Denominator & 0xF) << 00));
        }

        #endregion
    }
}
