﻿using System;
using System.IO;

namespace UnaryHeap.Utilities
{
    /// <summary>
    /// Represents a point in two-dimensional space.
    /// </summary>
    public class Point2D : IEquatable<Point2D>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the UnaryHeap.Utilities.Point2D class.
        /// </summary>
        /// <param name="x">The x-coordinate of the new point.</param>
        /// <param name="y">The y-coordinate of the new point.</param>
        /// <exception cref="System.ArgumentNullException">x or y are null references.</exception>
        public Point2D(Rational x, Rational y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value that represents the point (0, 0).
        /// </summary>
        public static Point2D Origin
        {
            get { throw new NotImplementedException(); }
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the x-cooridante of the current UnaryHeap.Utilities.Point2D object.
        /// </summary>
        public Rational X
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the y-cooridante of the current UnaryHeap.Utilities.Point2D object.
        /// </summary>
        public Rational Y
        {
            get { throw new NotImplementedException(); }
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified UnaryHeap.Utilities.Point2D object is equal to the current UnaryHeap.Utilities.Point2D object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the value of the obj parameter is equal to the value of the current UnaryHeap.Utilities.Point2D object; otherwise, false.</returns>
        public bool Equals(Point2D obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serves as a hash function for the UnaryHeap.Utilities.Point2D type.
        /// </summary>
        /// <returns>A hash code for the current UnaryHeap.Utilities.Point2D object.</returns>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the numeric value of the current UnaryHeap.Utilities.Point2D object to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the current UnaryHeap.Utilities.Point2D value.</returns>
        public override string ToString()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a binary representation of the current UnaryHeap.Utilities.Point2D value to a stream.
        /// </summary>
        /// <param name="output">The stream to which to write the binary representation.</param>
        /// <exception cref="System.ArgumentNullException">output is a null reference.</exception>
        public void Serialize(Stream output)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
