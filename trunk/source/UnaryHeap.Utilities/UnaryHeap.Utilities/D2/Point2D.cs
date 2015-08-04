using System;
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
        /// Initializes a new instance of the UnaryHeap.Utilities.Point2D class.
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

        #endregion


        #region Properties

        /// <summary>
        /// Gets the x-cooridante of the current UnaryHeap.Utilities.Point2D object.
        /// </summary>
        public Rational X
        {
            get { return x; }
        }

        /// <summary>
        /// Gets the y-cooridante of the current UnaryHeap.Utilities.Point2D object.
        /// </summary>
        public Rational Y
        {
            get { return y; }
        }

        #endregion


        #region Equality

        /// <summary>
        /// Determines whether the specified System.Object object is equal to the current UnaryHeap.Utilities.Point2D object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the obj parameter is a UnaryHeap.Utilities.Point2D object or a type capable of implicit
        /// conversion to a UnaryHeap.Utilities.Point2D value, and its value is equal to the value of the current
        /// UnaryHeap.Utilities.Point2D object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Point2D);
        }

        /// <summary>
        /// Determines whether the specified UnaryHeap.Utilities.Point2D object is equal to the current UnaryHeap.Utilities.Point2D object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the value of the obj parameter is equal to the value of the current UnaryHeap.Utilities.Point2D object; otherwise, false.</returns>
        public bool Equals(Point2D obj)
        {
            if (null == obj)
                return false;

            return this.x == obj.x && this.y == obj.y;
        }

        /// <summary>
        /// Serves as a hash function for the UnaryHeap.Utilities.Point2D type.
        /// </summary>
        /// <returns>A hash code for the current UnaryHeap.Utilities.Point2D object.</returns>
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
        /// Converts the string representation of a numeric value to its equivalent UnaryHeap.Utilities.Point2D object.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A UnaryHeap.Utilities.Point2D object with the current value.</returns>
        /// <exception cref="System.ArgumentNullException">value is a null reference.</exception>
        /// <exception cref="System.FormatException">Input string is not in a correct format.</exception>
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
        /// Converts the numeric value of the current UnaryHeap.Utilities.Point2D object to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the current UnaryHeap.Utilities.Point2D value.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1}", x, y);
        }

        #endregion


        #region Binary Serialization

        /// <summary>
        /// Reads the binary representation of a UnaryHeap.Utilities.Point2D object from a stream.
        /// </summary>
        /// <param name="input">The stream from which to read the binary representation.</param>
        /// <returns>The UnaryHeap.Utilities.Point2D value read.</returns>
        /// <exception cref="System.ArgumentNullException">input is a null reference.</exception>
        /// <exception cref="System.FormatException">data in intput stream could not be converted to a UnaryHeap.Utilities.Point2D object.</exception>
        public static Point2D Deserialize(Stream input)
        {
            if (null == input)
                throw new ArgumentNullException("input");

            var x = Rational.Deserialize(input);
            var y = Rational.Deserialize(input);

            return new Point2D(x, y);
        }

        /// <summary>
        /// Writes a binary representation of the current UnaryHeap.Utilities.Point2D value to a stream.
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
    }
}
