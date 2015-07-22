﻿using System;

namespace UnaryHeap.Utilities
{
    /// <summary>
    /// Represents a closed interval.
    /// </summary>
    public class Range
    {
        Rational min;
        Rational max;

        /// <summary>
        /// Initializes a new instance of the UnaryHeap.Utilities.Range class.
        /// </summary>
        /// <param name="min">The smallest value in the interval.</param>
        /// <param name="max">The largest value in the interval.</param>
        /// <exception cref="System.ArgumentNullException">min or max are null.</exception>
        /// <exception cref="System.ArgumentException">min is greater than max.</exception>
        public Range(Rational min, Rational max)
        {
            if (null == min)
                throw new ArgumentNullException("min");
            if (null == max)
                throw new ArgumentNullException("max");
            if (min > max)
                throw new ArgumentException("min is greater than max.");

            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Gets the smallest value in the interval.
        /// </summary>
        public Rational Min
        {
            get { return min; }
        }

        /// <summary>
        /// Gets the largest value in the interval.
        /// </summary>
        public Rational Max
        {
            get { return max; }
        }

        /// <summary>
        /// Gets the rational value halfway between Min and Max.
        /// </summary>
        public Rational Midpoint
        {
            get { return (min + max) / 2; }
        }

        /// <summary>
        /// Gets the difference between Max and Min.
        /// </summary>
        public Rational Size
        {
            get { return max - min; }
        }

        /// <summary>
        /// Determines whether the specified UnaryHeap.Utilities.Rational value lies within the current UnaryHeap.Utilities.Range.
        /// </summary>
        /// <param name="value">The value to check for membership.</param>
        /// <returns>True, if value is not less than Min or greater than Max; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        public bool Contains(Rational value)
        {
            if (null == value)
                throw new ArgumentNullException("value");

            return value >= min && value <= max;
        }

        /// <summary>
        /// Gets a new UnaryHeap.Utitiles.Range object whose endpoints are offset by a constant value
        /// from the current UnaryHeap.Utilities.Range object.
        /// </summary>
        /// <param name="thickness">The amount to offset.</param>
        /// <returns>A new UnaryHeap.Utilities.Range from Min - thickness to Max + thickness.</returns>
        /// <exception cref="System.ArgumentNullException">thickness is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">thickness is negative and more than half of Size.</exception>
        public Range GetPadded(Rational thickness)
        {
            if (null == thickness)
                throw new ArgumentNullException("thickness");
            if (Size < -2 * thickness)
                throw new ArgumentOutOfRangeException("thickness", "Specified thickness would result in a range with negative Size.");

            return new Range(min - thickness, max + thickness);
        }

        /// <summary>
        /// Gets a new UnaryHeap.Utitiles.Range object whose size is a multiple of
        /// the current UnaryHeap.Utitiles.Range instance.
        /// </summary>
        /// <param name="factor">The amount by which to scale the range of the current UnaryHeap.Utitiles.Range instance.</param>
        /// <returns>A new UnaryHeap.Utitiles.Range object with the same Midpoint as
        /// the current UnaryHeap.Utitiles.Range instance, and with a Size equal to the current
        /// UnaryHeap.Utitiles.Range instance's size multiplied by factor.</returns>
        /// <exception cref="System.ArgumentNullException">factor is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">factor is negative.</exception>
        public Range GetScaled(Rational factor)
        {
            if (null == factor)
                throw new ArgumentNullException("factor");
            if (0 > factor)
                throw new ArgumentOutOfRangeException("factor", "factor is negative.");

            var midpoint = Midpoint;
            var delta = Size * factor / 2;

            return new Range(midpoint - delta, midpoint + delta);
        }
    }
}