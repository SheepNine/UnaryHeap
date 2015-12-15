using System;
using System.Globalization;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Core;

namespace UnaryHeap.Utilities.D3
{
    /// <summary>
    /// Represents a point in three-dimensional space.
    /// </summary>
    public class Point3D : IEquatable<Point3D>
    {
        #region Member Variables

        Rational x;
        Rational y;
        Rational z;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Point3D class.
        /// </summary>
        /// <param name="x">The x-coordinate of the new point.</param>
        /// <param name="y">The y-coordinate of the new point.</param>
        /// <param name="z">The z-coordinate of the new point.</param>
        /// <exception cref="System.ArgumentNullException">
        /// x, y or z are null references.</exception>
        public Point3D(Rational x, Rational y, Rational z)
        {
            if (null == x)
                throw new ArgumentNullException("x");
            if (null == y)
                throw new ArgumentNullException("y");
            if (null == z)
                throw new ArgumentNullException("z");

            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Gets a value that represents the point (0, 0, 0).
        /// </summary>
        public static Point3D Origin
        {
            get { return new Point3D(0, 0, 0); }
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the x-cooridante of the current Point3D object.
        /// </summary>
        public Rational X
        {
            get { return x; }
        }

        /// <summary>
        /// Gets the y-cooridante of the current Point3D object.
        /// </summary>
        public Rational Y
        {
            get { return y; }
        }

        /// <summary>
        /// Gets the z-cooridante of the current Point3D object.
        /// </summary>
        public Rational Z
        {
            get { return z; }
        }

        #endregion


        #region Equality

        /// <summary>
        /// Determines whether the specified System.Object object is equal to
        /// the current Point3D object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the obj parameter is a Point3D object or a type capable of implicit
        /// conversion to a Point3D value, and its value is equal to the value of the current
        /// Point3D object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Point3D);
        }

        /// <summary>
        /// Determines whether the specified Point3D object is equal to the
        /// current Point3D object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>true if the value of the obj parameter is equal to the value of
        /// the current Point3D object; otherwise, false.</returns>
        public bool Equals(Point3D other)
        {
            if (null == other)
                return false;

            return this.x == other.x
                && this.y == other.y
                && this.z == other.z;
        }

        /// <summary>
        /// Serves as a hash function for the Point3D type.
        /// </summary>
        /// <returns>A hash code for the current Point3D object.</returns>
        public override int GetHashCode()
        {
            return (int)
                (((x.Numerator & 0x1F) << 25) |
                 ((x.Denominator & 0x1F) << 20) |
                 ((y.Numerator & 0x1F) << 15) |
                 ((y.Denominator & 0x1F) << 10) |
                 ((z.Numerator & 0x1F) << 05) |
                 ((z.Denominator & 0x1F) << 00));
        }

        #endregion


        #region Text Serialization

        /// <summary>
        /// Converts the string representation of a numeric value to its
        /// equivalent Point3D object.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A Point3D object with the current value.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// value is a null reference.</exception>
        /// <exception cref="System.FormatException">
        /// Input string is not in a correct format.</exception>
        public static Point3D Parse(string value)
        {
            if (null == value)
                throw new ArgumentNullException("value");

            value = value.Trim();

            if (value.Any(c => char.IsWhiteSpace(c)))
                throw new FormatException("Input string was not in a correct format.");

            var tokens = value.Split(',');

            if (tokens.Length != 3)
                throw new FormatException("Input string was not in a correct format.");

            return new Point3D(
                Rational.Parse(tokens[0]),
                Rational.Parse(tokens[1]),
                Rational.Parse(tokens[2]));
        }

        /// <summary>
        /// Converts the numeric value of the current Point3D object to its equivalent
        /// string representation.
        /// </summary>
        /// <returns>The string representation of the current Point3D value.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", x, y,z);
        }

        #endregion


        #region Binary Serialization

        /// <summary>
        /// Reads the binary representation of a Point3D object from a stream.
        /// </summary>
        /// <param name="input">The stream from which to read the binary representation.</param>
        /// <returns>The Point3D value read.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// input is a null reference.</exception>
        /// <exception cref="System.FormatException">
        /// data in intput stream could not be converted to a Point3D object.</exception>
        public static Point3D Deserialize(Stream input)
        {
            if (null == input)
                throw new ArgumentNullException("input");

            var x = Rational.Deserialize(input);
            var y = Rational.Deserialize(input);
            var z = Rational.Deserialize(input);

            return new Point3D(x, y, z);
        }

        /// <summary>
        /// Writes a binary representation of the current Point3D value to a stream.
        /// </summary>
        /// <param name="output">The stream to which to write the binary representation.</param>
        /// <exception cref="System.ArgumentNullException">
        /// output is a null reference.</exception>
        public void Serialize(Stream output)
        {
            if (null == output)
                throw new ArgumentNullException("output");

            x.Serialize(output);
            y.Serialize(output);
            z.Serialize(output);
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Computes the squared distance between two points.
        /// </summary>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <returns>The squared distance between the two points.</returns>
        public static Rational Quadrance(Point3D p1, Point3D p2)
        {
            if (null == p1)
                throw new ArgumentNullException("p1");
            if (null == p2)
                throw new ArgumentNullException("p2");

            var dx = p1.x - p2.x;
            var dy = p1.y - p2.y;
            var dz = p1.z - p2.z;

            return dx.Squared + dy.Squared + dz.Squared;
        }

        #endregion
    }
}