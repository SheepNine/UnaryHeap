using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Core;

namespace UnaryHeap.Utilities.D2
{
    /// <summary>
    /// Represents a point in two-dimensional space.
    /// </summary>
    public class Point2D : IEquatable<Point2D>
    {
        #region Member Variables

        Rational x;
        Rational y;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Point2D class.
        /// </summary>
        /// <param name="x">The x-coordinate of the new point.</param>
        /// <param name="y">The y-coordinate of the new point.</param>
        /// <exception cref="System.ArgumentNullException">x or y are null references.</exception>
        public Point2D(Rational x, Rational y)
        {
            if (null == x)
                throw new ArgumentNullException("x");
            if (null == y)
                throw new ArgumentNullException("y");

            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Gets a value that represents the point (0, 0).
        /// </summary>
        public static Point2D Origin
        {
            get { return new Point2D(0, 0); }
        }

        /// <summary>
        /// Computes the circumcenter of a given triple of points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <returns>The circumcenter of a,b, and c, or null, if
        /// the points are colinear.</returns>
        /// <exception cref="System.ArgumentNullException">a, b, or c are null.</exception>
        public static Point2D Circumcenter(Point2D a, Point2D b, Point2D c)
        {
            if (null == a)
                throw new ArgumentNullException("a");
            if (null == b)
                throw new ArgumentNullException("b");
            if (null == c)
                throw new ArgumentNullException("c");

            var G = 2 * (a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y));

            if (0 == G)
                return null;

            var aQ = a.X.Squared + a.Y.Squared;
            var bQ = b.X.Squared + b.Y.Squared;
            var cQ = c.X.Squared + c.Y.Squared;

            return new Point2D(
                (aQ * (b.Y - c.Y) + bQ * (c.Y - a.Y) + cQ * (a.Y - b.Y)) / G,
                (cQ * (b.X - a.X) + bQ * (a.X - c.X) + aQ * (c.X - b.X)) / G);
        }

        /// <summary>
        /// Generates a set of points randomly distributed in a square area.
        /// </summary>
        /// <param name="numPoints">The number of points to generate.</param>
        /// <param name="seed">The random number seed, or null to use the default seed.</param>
        /// <returns>A set of points randomly distributed in a square area.</returns>
        public static Point2D[] GenerateRandomPoints(int numPoints, int? seed = null)
        {
            //TODO: Find a new home for this method; it is more general than Fortune's algorithm.
            if (numPoints < 2)
                throw new ArgumentOutOfRangeException("numPoints");

            var random = seed.HasValue ? new Random(seed.Value) : new Random();
            var yValues = Enumerable.Range(0, numPoints).ToList();

            var result = new List<Point2D>();

            for (int x = 0; x < numPoints; x++)
            {
                var index = random.Next(yValues.Count);
                var y = yValues[index];
                yValues.RemoveAt(index);

                result.Add(new Point2D(x, y));
            }

            return result.ToArray();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the x-cooridante of the current Point2D object.
        /// </summary>
        public Rational X
        {
            get { return x; }
        }

        /// <summary>
        /// Gets the y-cooridante of the current Point2D object.
        /// </summary>
        public Rational Y
        {
            get { return y; }
        }

        #endregion


        #region Equality

        /// <summary>
        /// Determines whether the specified System.Object object is equal to the current
        /// Point2D object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the obj parameter is a Point2D object or a type capable of implicit
        /// conversion to a Point2D value, and its value is equal to the value of the current
        /// Point2D object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Point2D);
        }

        /// <summary>
        /// Determines whether the specified Point2D object is equal to the current
        /// Point2D object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>true if the value of the obj parameter is equal to the value
        /// of the current Point2D object; otherwise, false.</returns>
        public bool Equals(Point2D other)
        {
            if (null == other)
                return false;

            return this.x == other.x && this.y == other.y;
        }

        /// <summary>
        /// Serves as a hash function for the Point2D type.
        /// </summary>
        /// <returns>A hash code for the current Point2D object.</returns>
        public override int GetHashCode()
        {
            return (int)
                (((x.Numerator & 0xFFF) << 5) +
                ((x.Denominator & 0xFF) << 4) +
                ((y.Numerator & 0xFFF) << 1) +
                (y.Denominator & 0xF));
        }

        #endregion


        #region Text Serialization

        /// <summary>
        /// Converts the string representation of a numeric value to its equivalent
        /// Point2D object.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A Point2D object with the current value.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// value is a null reference.</exception>
        /// <exception cref="System.FormatException">
        /// Input string is not in a correct format.</exception>
        public static Point2D Parse(string value)
        {
            if (null == value)
                throw new ArgumentNullException("value");

            value = value.Trim();

            if (value.Any(c => char.IsWhiteSpace(c)))
                throw new FormatException("Input string was not in a correct format.");

            var tokens = value.Split(',');

            if (tokens.Length != 2)
                throw new FormatException("Input string was not in a correct format.");

            return new Point2D(Rational.Parse(tokens[0]), Rational.Parse(tokens[1]));
        }

        /// <summary>
        /// Converts the numeric value of the current Point2D object to its equivalent
        /// string representation.
        /// </summary>
        /// <returns>The string representation of the current Point2D value.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1}", x, y);
        }

        #endregion


        #region Binary Serialization

        /// <summary>
        /// Reads the binary representation of a Point2D object from a stream.
        /// </summary>
        /// <param name="input">The stream from which to read the binary representation.</param>
        /// <returns>The Point2D value read.</returns>
        /// <exception cref="System.ArgumentNullException">input is a null reference.</exception>
        /// <exception cref="System.FormatException">
        /// data in intput stream could not be converted to a Point2D object.</exception>
        public static Point2D Deserialize(Stream input)
        {
            if (null == input)
                throw new ArgumentNullException("input");

            var x = Rational.Deserialize(input);
            var y = Rational.Deserialize(input);

            return new Point2D(x, y);
        }

        /// <summary>
        /// Writes a binary representation of the current Point2D value to a stream.
        /// </summary>
        /// <param name="output">The stream to which to write the binary representation.</param>
        /// <exception cref="System.ArgumentNullException">output is a null reference.</exception>
        public void Serialize(Stream output)
        {
            if (null == output)
                throw new ArgumentNullException("output");

            x.Serialize(output);
            y.Serialize(output);
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Computes the squared distance between two points.
        /// </summary>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <returns>The squared distance between the two points.</returns>
        public static Rational Quadrance(Point2D p1, Point2D p2)
        {
            if (null == p1)
                throw new ArgumentNullException("p1");
            if (null == p2)
                throw new ArgumentNullException("p2");

            var dx = p1.x - p2.x;
            var dy = p1.y - p2.y;

            return dx.Squared + dy.Squared;
        }

        #endregion
    }
}
