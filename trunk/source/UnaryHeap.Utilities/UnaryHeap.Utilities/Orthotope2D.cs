#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Collections.Generic;

namespace UnaryHeap.Utilities
{
    /// <summary>
    /// Represents a two-dimensional closed interval; ie, a rectangle.
    /// </summary>
    public class Orthotope2D
    {
        /// <summary>
        /// Intializes a new instance of the UnaryHeap.Utilities.Orthotope2D class.
        /// </summary>
        /// <param name="x">The x-interval of the new instance.</param>
        /// <param name="y">The x-interval of the new instance.</param>
        /// <exception cref="System.ArgumentNullException">x or y are null.</exception>
        public Orthotope2D(Range x, Range y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Intializes a new instance of the UnaryHeap.Utilities.Orthotope2D class.
        /// </summary>
        /// <param name="minX">The smallest value in the x interval.</param>
        /// <param name="minY">The smallest value in the y interval.</param>
        /// <param name="maxX">The largest value in the x interval.</param>
        /// <param name="maxY">The largest value in the y interval.</param>
        /// <exception cref="System.ArgumentNullException">minX, minY, maxX or maxY are null.</exception>
        /// <exception cref="System.ArgumentException">minX is greater than maxX, or minY is greater than maxY.</exception>
        public Orthotope2D(Rational minX, Rational minY, Rational maxX, Rational maxY)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the UnaryHeap.Utilities.Orthotope2D class as the bounding box of a set of points.
        /// </summary>
        /// <param name="points">The points from which to compute the bounding box.</param>
        /// <returns>A new UnaryHeap.Utilities.Orthotope2D object with the smallest range containing all of the input points.</returns>
        /// <exception cref="System.ArgumentNullException">points is null.</exception>
        /// <exception cref="System.ArgumentException">points is empty, or contains a null value.</exception>
        public static Orthotope2D FromPoints(IEnumerable<Point2D> points)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the x-axis interval.
        /// </summary>
        public Range X
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the y-axis interval.
        /// </summary>
        public Range Y
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Determines whether the specified UnaryHeap.Utilities.Point2D value lies within the current UnaryHeap.Utilities.Orthotope2D.
        /// </summary>
        /// <param name="value">The value to check for membership.</param>
        /// <returns>True, if value.X lies within the X range and value.Y lies within the Y range.</returns>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        public bool Contains(Point2D value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a new UnaryHeap.Utilities.Orthotope2D object whose endpoints wre offset from the current UnaryHeap.Utilities.Orthotope2D instance.
        /// </summary>
        /// <param name="thickness">The amount by which to offset.</param>
        /// <returns>A new UnaryHeap.Utilities.Orthotope2D object with each axis padded by the specified amount.</returns>
        /// <exception cref="System.ArgumentNullException">thickness is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thickness is negative and more than half of X.Size or Y.Size.</exception>
        public Orthotope2D GetPadded(Rational thickness)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a new UnaryHeap.Utilities.Orthotope2D object whose size is a multiple of the current UnaryHeap.Utilities.Orthotope2D instance.
        /// </summary>
        /// <param name="factor">The amount by which to scale.</param>
        /// <returns>A new UnaryHeap.Utilities.Orthotope2D object with each axis scaled by the specified amount.</returns>
        /// <exception cref="System.ArgumentNullException">factor is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">factor is negative.</exception>
        public Orthotope2D GetScaled(Rational factor)
        {
            throw new NotImplementedException();
        }
    }
}

#endif