using System;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a parabola in a two-dimensional coordinate system
    /// with a vertical axis of symmetry.
    /// </summary>
    public class Parabola
    {
        /// <summary>
        /// Gets the 2nd-order coefficient of this Parabola.
        /// </summary>
        public Rational A { get; private set; }

        /// <summary>
        /// Gets the 1st-order coefficient of this Parabola.
        /// </summary>
        public Rational B { get; private set; }

        /// <summary>
        /// Gets the 0th-order coefficient of this Parabola.
        /// </summary>
        public Rational C { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Parabola class.
        /// </summary>
        /// <param name="a">The 2nd-order coefficient. This must be non-zero.</param>
        /// <param name="b">The 1st-order coefficient.</param>
        /// <param name="c">The 0th-order coefficient.</param>
        /// <exception cref="System.ArgumentNullException">a, b, or c are null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">a is zero.</exception>
        public Parabola(Rational a, Rational b, Rational c)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            ArgumentNullException.ThrowIfNull(c);

            if (Rational.Zero == a)
                throw new ArgumentOutOfRangeException(nameof(a),
                    "Highest-order coefficient is zero");

            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// Initalizes a new instance of the Parabola class with the specified
        /// focus and directrix.
        /// </summary>
        /// <param name="focus">The focus point.</param>
        /// <param name="directrixY">The y-value of the horizontal directrix.</param>
        /// <returns>A parabola with the specified focus and directrix.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// focus or directrix are null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The focus Y coordinate is equal to directrixY.</exception>
        public static Parabola FromFocusDirectrix(Point2D focus, Rational directrixY)
        {
            ArgumentNullException.ThrowIfNull(focus);
            ArgumentNullException.ThrowIfNull(directrixY);
            if (focus.Y == directrixY)
                throw new ArgumentException("Focus is on the directrix.");

            var a = 1;
            var b = -2 * focus.X;
            var c = (focus.X.Squared + focus.Y.Squared - directrixY * directrixY);
            var d = 2 * (focus.Y - directrixY);

            return new Parabola(a / d, b / d, c / d);
        }

        /// <summary>
        /// Calculates the parabola formed by the difference between two other parabolas.
        /// </summary>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend).</param>
        /// <returns>The result of subtracting right from left.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// left or right are null references.</exception>
        /// <exception cref="System.ArgumentException">
        /// Left and right have the same 2nd-order coefficient.</exception>
        public static Parabola Difference(Parabola left, Parabola right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            if (left.A == right.A)
                throw new ArgumentException(
                    "Parabolas have same highest-order coefficient, " +
                    "their difference is linear, not parabolic.");

            return new Parabola(left.A - right.A, left.B - right.B, left.C - right.C);
        }

        /// <summary>
        /// Calculates the Y-value of the parabola at the specified X value.
        /// </summary>
        /// <param name="x">The X value at which to evaluate the parabola.</param>
        /// <returns>The Y-value of the parabola at the specified value.</returns>
        /// <exception cref="System.ArgumentNullException">x is null.</exception>
        public Rational Evaulate(Rational x)
        {
            ArgumentNullException.ThrowIfNull(x);

            return C + x * (B + x * A);
        }

        /// <summary>
        /// Calculates the derivative of the parabola at the specified X value.
        /// </summary>
        /// <param name="x">The X value at which to evaluate the parabola's derivative.</param>
        /// <returns>The derivative of the parabola at the specified value.</returns>
        /// <exception cref="System.ArgumentNullException">x is null.</exception>
        public Rational EvaluateDerivative(Rational x)
        {
            ArgumentNullException.ThrowIfNull(x);

            return B + 2 * A * x;
        }

        /// <summary>
        /// Gets the focus of this Parabola.
        /// </summary>
        public Point2D Focus
        {
            get
            {
                var det = B * B - 4 * A * C;
                return new Point2D(-B / (2 * A), (1 - det) / (4 * A));
            }
        }

        /// <summary>
        /// Gets the Y-value of the (horizontal) directrix of this Parabola.
        /// </summary>
        public Rational DirectrixY
        {
            get
            {
                var det = B * B - 4 * A * C;
                return (-1 - det) / (4 * A);
            }
        }
    }
}
