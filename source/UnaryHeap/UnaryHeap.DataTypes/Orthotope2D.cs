﻿using System;
using System.Collections.Generic;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a two-dimensional closed interval; ie, a rectangle.
    /// </summary>
    public class Orthotope2D
    {
        Range x;
        Range y;

        /// <summary>
        /// Intializes a new instance of the Orthotope2D class.
        /// </summary>
        /// <param name="x">The x-interval of the new instance.</param>
        /// <param name="y">The x-interval of the new instance.</param>
        /// <exception cref="System.ArgumentNullException">x or y are null.</exception>
        public Orthotope2D(Range x, Range y)
        {
            if (null == x)
                throw new ArgumentNullException(nameof(x));
            if (null == y)
                throw new ArgumentNullException(nameof(y));

            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Intializes a new instance of the Orthotope2D class.
        /// </summary>
        /// <param name="minX">The smallest value in the x interval.</param>
        /// <param name="minY">The smallest value in the y interval.</param>
        /// <param name="maxX">The largest value in the x interval.</param>
        /// <param name="maxY">The largest value in the y interval.</param>
        /// <exception cref="System.ArgumentNullException">
        /// minX, minY, maxX or maxY are null.</exception>
        /// <exception cref="System.ArgumentException">
        /// minX is greater than maxX, or minY is greater than maxY.</exception>
        public Orthotope2D(Rational minX, Rational minY, Rational maxX, Rational maxY)
            : this(new Range(minX, maxX), new Range(minY, maxY))
        {
        }

        /// <summary>
        /// Initializes a new instance of the Orthotope2D class as the bounding box
        /// of a set of points.
        /// </summary>
        /// <param name="points">The points from which to compute the bounding box.</param>
        /// <returns>A new Orthotope2D object with the smallest range containing
        /// all of the input points.</returns>
        /// <exception cref="System.ArgumentNullException">points is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// points is empty, or contains a null value.</exception>
        public static Orthotope2D FromPoints(IEnumerable<Point2D> points)
        {
            if (null == points)
                throw new ArgumentNullException(nameof(points));

            Rational minX = null;
            Rational minY = null;
            Rational maxX = null;
            Rational maxY = null;
            bool initialized = false;

            foreach (var value in points)
            {
                if (null == value)
                    throw new ArgumentNullException(nameof(points));

                if (null == minX)
                {
                    minX = value.X;
                    minY = value.Y;
                    maxX = value.X;
                    maxY = value.Y;
                    initialized = true;
                }
                else
                {
                    minX = Rational.Min(minX, value.X);
                    minY = Rational.Min(minY, value.Y);
                    maxX = Rational.Max(maxX, value.X);
                    maxY = Rational.Max(maxY, value.Y);
                }
            }

            if (false == initialized)
                throw new ArgumentException("Enumerable is empty.", nameof(points));

            return new Orthotope2D(minX, minY, maxX, maxY);
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
        /// Gets the Point2D of the center of the current Orthotope2D.
        /// </summary>
        public Point2D Center
        {
            get { return new Point2D(X.Midpoint, Y.Midpoint); }
        }

        /// <summary>
        /// Determines whether the specified Point2D value lies within the
        /// current Orthotope2D.
        /// </summary>
        /// <param name="value">The value to check for membership.</param>
        /// <returns>True, if value.X lies within the X range and value.Y
        /// lies within the Y range.</returns>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        public bool Contains(Point2D value)
        {
            if (null == value)
                throw new ArgumentNullException(nameof(value));

            return x.Contains(value.X) && y.Contains(value.Y);
        }

        /// <summary>
        /// Gets a new Orthotope2D object whose endpoints wre offset from the
        /// current Orthotope2D instance.
        /// </summary>
        /// <param name="thickness">The amount by which to offset.</param>
        /// <returns>A new Orthotope2D object with each axis padded by
        /// the specified amount.</returns>
        /// <exception cref="System.ArgumentNullException">thickness is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// thickness is negative and more than half of X.Size or Y.Size.</exception>
        public Orthotope2D GetPadded(Rational thickness)
        {
            return new Orthotope2D(x.GetPadded(thickness), y.GetPadded(thickness));
        }

        /// <summary>
        /// Gets a new Orthotope2D object whose size is a multiple of the
        /// current Orthotope2D instance.
        /// </summary>
        /// <param name="factor">The amount by which to scale.</param>
        /// <returns>A new Orthotope2D object with each axis scaled by
        /// the specified amount.</returns>
        /// <exception cref="System.ArgumentNullException">factor is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// factor is negative.</exception>
        public Orthotope2D GetScaled(Rational factor)
        {
            return new Orthotope2D(x.GetScaled(factor), y.GetScaled(factor));
        }

        /// <summary>
        /// Gets a new Orthotope2D object with the same area whose center
        /// is equal to the center specified.
        /// </summary>
        /// <param name="center">The center of the new Orthotope2D.</param>
        /// <returns>A new Orthotope2D object with the same area whose center
        /// is equal to the center specified.</returns>
        /// <exception cref="System.ArgumentNullException">center is null.
        /// </exception>
        public Orthotope2D CenteredAt(Point2D center)
        {
            if (null == center)
                throw new ArgumentNullException(nameof(center));

            return new Orthotope2D(x.CenteredAt(center.X), y.CenteredAt(center.Y));
        }
    }
}