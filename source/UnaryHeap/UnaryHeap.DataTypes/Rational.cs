using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents an arbitrarily large signed rational number.
    /// </summary>
    public class Rational : IComparable, IComparable<Rational>, IEquatable<Rational>
    {
        #region Member Variables

        BigInteger numerator;
        BigInteger denominator;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Rational class from the given integeral value.
        /// </summary>
        /// <param name="value">The value to initialize the new instance to.</param>
        public Rational(BigInteger value)
            : this(value, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Rational class from the given integral
        /// numerator and denominator.
        /// </summary>
        /// <param name="numerator">The numerator of the new instance.</param>
        /// <param name="denominator">The denominator of the new instance.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Denominator is zero.</exception>
        public Rational(BigInteger numerator, BigInteger denominator)
            : this(numerator, denominator, true)
        {
        }

        Rational(BigInteger numerator, BigInteger denominator, bool reduce)
        {
            var sign = denominator.Sign;

            if (sign > 0)
            {
                this.numerator = numerator;
                this.denominator = denominator;
            }
            else if (sign < 0)
            {
                this.numerator = -numerator;
                this.denominator = -denominator;
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    nameof(denominator), "Denominator cannot be zero.");
            }

            if (reduce)
            {
                var gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);

                if (false == gcd.IsOne)
                {
                    this.numerator = this.numerator / gcd;
                    this.denominator = this.denominator / gcd;
                }
            }
        }

        #region Conversion Operators

        /// <summary>
        /// Defines an implicit conversion of a System.Int32 object to a Rational value.
        /// </summary>
        /// <param name="value">The value to convert to a Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(int value)
        {
            return new Rational(value);
        }

        /// <summary>
        /// Defines an implicit conversion of a System.UInt32 object to a Rational value.
        /// </summary>
        /// <param name="value">The value to convert to a Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(uint value)
        {
            return new Rational(value);
        }

        /// <summary>
        /// Defines an implicit conversion of a System.Int64 object to a Rational value.
        /// </summary>
        /// <param name="value">The value to convert to a Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(long value)
        {
            return new Rational(value);
        }

        /// <summary>
        /// Defines an implicit conversion of a System.UInt64 object to a Rational value.
        /// </summary>
        /// <param name="value">The value to convert to a Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(ulong value)
        {
            return new Rational(value);
        }

        /// <summary>
        /// Defines an implicit conversion of a System.Int16 object to a Rational value.
        /// </summary>
        /// <param name="value">The value to convert to a Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(short value)
        {
            return new Rational(value);
        }

        /// <summary>
        /// Defines an implicit conversion of a System.UInt16 object to a Rational value.
        /// </summary>
        /// <param name="value">The value to convert to a Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(ushort value)
        {
            return new Rational(value);
        }

        /// <summary>
        /// Defines an implicit conversion of a System.SByte object to a Rational value.
        /// </summary>
        /// <param name="value">The value to convert to a Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(sbyte value)
        {
            return new Rational(value);
        }

        /// <summary>
        /// Defines an implicit conversion of a System.Byte object to a Rational value.
        /// </summary>
        /// <param name="value">The value to convert to a Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(byte value)
        {
            return new Rational(value);
        }

        /// <summary>
        /// Defines an implicit conversion of a System.Numerics.BigInteger object to
        /// a Rational value.
        /// </summary>
        /// <param name="value">The value to convert to a Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(BigInteger value)
        {
            return new Rational(value);
        }

        /// <summary>
        /// Defines an explicit conversion of a Rational object to a System.Double value.
        /// </summary>
        /// <param name="value">The value to convert to a System.Double.</param>
        /// <returns>A double that contains the value of the value parameter.</returns>
        public static explicit operator double(Rational value)
        {
            if (object.ReferenceEquals(null, value))
                throw new ArgumentNullException(nameof(value));

            if (value.numerator.IsZero)
                return 0.0;

            var resultNegative = value.numerator.Sign < 0;
            var numerator = BigInteger.Abs(value.numerator);
            var denominator = value.denominator;


            // --- Adjust numerator and denominator and exponent so that the resulting mantissa
            // --- is between 1 and 2 ---

            // Valid range from 1 to 2046; 0 and 2047 are reserved
            var exponent = new BigInteger(1023);

            while (numerator > denominator)
            {
                denominator <<= 1;
                exponent += 1;

                if (exponent == 0x7FF)
                    throw new OverflowException("Value is too large to convert to double");
            }

            while (denominator > numerator)
            {
                numerator <<= 1;
                exponent -= 1;

                if (exponent.IsZero)
                    throw new OverflowException("Value is too small to convert to double");
            }


            // --- Compute mantissa ---

            var mantissaBits = (numerator << 53) / denominator;


            // --- Round up if necessary ---

            if (!mantissaBits.IsEven)
                mantissaBits++;

            mantissaBits >>= 1;


            // --- Pack the result ---

            var resultBits = mantissaBits & 0xFFFFFFFFFFFFF;

            resultBits += (exponent << 52);

            if (resultNegative)
                resultBits += 0x08000000000000000;

            return BitConverter.ToDouble(resultBits.ToByteArray(), 0);
        }

        #endregion


        #region Named Constants

        /// <summary>
        /// Gets a value that represents the number one.
        /// </summary>
        public static Rational One
        {
            get { return new Rational(1); }
        }

        /// <summary>
        /// Gets a value that represents the number zero.
        /// </summary>
        public static Rational Zero
        {
            get { return new Rational(0); }
        }

        /// <summary>
        /// Gets a value that represents the number negative one.
        /// </summary>
        public static Rational MinusOne
        {
            get { return new Rational(-1); }
        }

        #endregion

        #endregion


        #region Properties

        /// <summary>
        /// Gets the numerator of the current Rational object.
        /// </summary>
        public BigInteger Numerator
        {
            get { return numerator; }
        }

        /// <summary>
        /// Gets the denominator of the current Rational object.
        /// </summary>
        public BigInteger Denominator
        {
            get { return denominator; }
        }

        /// <summary>
        /// Gets a number that indicates the sign (negative, positive, or zero) of the current
        /// Rational object.
        /// </summary>
        public int Sign
        {
            get { return numerator.Sign; }
        }

        /// <summary>
        /// Gets the absolute value of the current Rational object.
        /// </summary>
        public Rational AbsoluteValue
        {
            get { return new Rational(BigInteger.Abs(numerator), denominator, false); }
        }

        /// <summary>
        /// Gets the largest whole number less than or equal to the current Rational object.
        /// </summary>
        public Rational Floor
        {
            get
            {
                if (denominator.IsOne)
                    return this;

                BigInteger rem;
                BigInteger.DivRem(numerator, denominator, out rem);

                if (rem < 0)
                    rem += denominator;

                return new Rational(numerator - rem, denominator);
            }
        }

        /// <summary>
        /// Gets the smallest whole number greater than or equal to the current Rational object.
        /// </summary>
        public Rational Ceiling
        {
            get
            {
                if (denominator.IsOne)
                    return this;

                BigInteger rem;
                BigInteger.DivRem(numerator, denominator, out rem);

                if (rem < 0)
                    rem += denominator;

                return new Rational(numerator + (denominator - rem), denominator);
            }
        }

        /// <summary>
        /// Gets the value of the current Rational object raised to the second power.
        /// </summary>
        public Rational Squared
        {
            get
            {
                return new Rational(
                    numerator * numerator, denominator * denominator, false);
            }
        }

        /// <summary>
        /// Gets the closest whole number to the current Rational object. If the current object
        /// is halfway between two whole numbers, the even number is returned.
        /// </summary>
        public Rational Rounded
        {
            get
            {
                if (denominator.IsOne)
                    return this;

                BigInteger rem;
                var whole = BigInteger.DivRem(numerator, denominator, out rem);

                var discriminant = 2 * BigInteger.Abs(rem);

                if (discriminant > denominator)
                    return new Rational(whole + numerator.Sign);
                else if (discriminant < denominator)
                    return new Rational(whole);
                else
                {
                    if (whole.IsEven)
                        return new Rational(whole);
                    else
                        return new Rational(whole + numerator.Sign);
                }
            }
        }

        /// <summary>
        /// Gets the multiplicative inverse of the current Rational object.
        /// </summary>
        /// <exception cref="System.DivideByZeroException">
        /// The current object equals Rational.Zero.</exception>
        public Rational Inverse
        {
            get
            {
                if (numerator.IsZero)
                    throw new InvalidOperationException("Zero does not have an inverse");

                return new Rational(
                    denominator * numerator.Sign, numerator * numerator.Sign, false);
            }
        }

        #endregion


        #region Operators

        /// <summary>
        /// Adds the values of two specified Rational objects.
        /// </summary>
        /// <param name="left">The first value to add.</param>
        /// <param name="right">The second value to add.</param>
        /// <returns>The sum of left and right.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// left or right are null references.</exception>
        public static Rational Add(Rational left, Rational right)
        {
            return left + right;
        }

        /// <summary>
        /// Adds the values of two specified Rational objects.
        /// </summary>
        /// <param name="left">The first value to add.</param>
        /// <param name="right">The second value to add.</param>
        /// <returns>The sum of left and right.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// left or right are null references.</exception>
        public static Rational operator +(Rational left, Rational right)
        {
            if (object.ReferenceEquals(left, null))
                throw new ArgumentNullException(nameof(left));
            if (object.ReferenceEquals(right, null))
                throw new ArgumentNullException(nameof(right));

            if (left.numerator.IsZero)
                return right;
            if (right.numerator.IsZero)
                return left;
            if (left.denominator == right.denominator)
                return new Rational(left.numerator + right.numerator, left.denominator);
            if (left.denominator.IsOne)
                return new Rational(
                    left.numerator * right.denominator + right.numerator, right.denominator,
                    false);
            if (right.denominator.IsOne)
                return new Rational(
                    left.numerator + right.numerator * left.denominator, left.denominator,
                    false);

            return new Rational(
                left.numerator * right.denominator + right.numerator * left.denominator,
                left.denominator * right.denominator);
        }

        /// <summary>
        /// Subtracts a Rational value from another Rational value.
        /// </summary>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend).</param>
        /// <returns>The result of subtracting right from left.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// left or right are null references.</exception>
        public static Rational Subtract(Rational left, Rational right)
        {
            return left - right;
        }

        /// <summary>
        /// Subtracts a Rational value from another Rational value.
        /// </summary>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend).</param>
        /// <returns>The result of subtracting right from left.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// left or right are null references.</exception>
        public static Rational operator -(Rational left, Rational right)
        {
            if (object.ReferenceEquals(left, null))
                throw new ArgumentNullException(nameof(left));
            if (object.ReferenceEquals(right, null))
                throw new ArgumentNullException(nameof(right));

            if (left.numerator.IsZero)
                return -right;
            if (right.numerator.IsZero)
                return left;
            if (left.denominator == right.denominator)
                return new Rational(left.numerator - right.numerator, left.denominator);
            if (left.denominator.IsOne)
                return new Rational(
                    left.numerator * right.denominator - right.numerator, right.denominator,
                    false);
            if (right.denominator.IsOne)
                return new Rational(
                    left.numerator - right.numerator * left.denominator, left.denominator,
                    false);

            return new Rational(
                left.numerator * right.denominator - right.numerator * left.denominator,
                left.denominator * right.denominator);
        }

        /// <summary>
        /// Multiplies two specified Rational values.
        /// </summary>
        /// <param name="left">The first value to multiply.</param>
        /// <param name="right">The second value to multiply.</param>
        /// <returns>The product of left and right.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// left or right are null references.</exception>
        public static Rational Multiply(Rational left, Rational right)
        {
            return left * right;
        }

        /// <summary>
        /// Multiplies two specified Rational values.
        /// </summary>
        /// <param name="left">The first value to multiply.</param>
        /// <param name="right">The second value to multiply.</param>
        /// <returns>The product of left and right.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// left or right are null references.</exception>
        public static Rational operator *(Rational left, Rational right)
        {
            if (object.ReferenceEquals(left, null))
                throw new ArgumentNullException(nameof(left));
            if (object.ReferenceEquals(right, null))
                throw new ArgumentNullException(nameof(right));

            if (left.numerator.IsZero)
                return Zero;
            if (right.numerator.IsZero)
                return Zero;

            return new Rational(
                left.numerator * right.numerator, left.denominator * right.denominator);
        }

        /// <summary>
        /// Divides a specified Rational value by another specified Rational value by
        /// using integer division.
        /// </summary>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <returns>The integral result of the division.</returns>
        /// <exception cref="System.DivideByZeroException">divisor is zero.</exception>
        /// <exception cref="System.ArgumentNullException">
        /// dividend or divisor are null references.</exception>
        public static Rational Divide(Rational dividend, Rational divisor)
        {
            return dividend / divisor;
        }

        /// <summary>
        /// Divides a specified Rational value by another specified Rational
        /// value by using integer division.
        /// </summary>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <returns>The integral result of the division.</returns>
        /// <exception cref="System.DivideByZeroException">divisor is zero.</exception>
        /// <exception cref="System.ArgumentNullException">
        /// dividend or divisor are null references.</exception>
        public static Rational operator /(Rational dividend, Rational divisor)
        {
            if (object.ReferenceEquals(dividend, null))
                throw new ArgumentNullException(nameof(dividend));
            if (object.ReferenceEquals(divisor, null))
                throw new ArgumentNullException(nameof(divisor));

            if (dividend.numerator.IsZero)
                return Zero;
            if (divisor.numerator.IsZero)
                throw new DivideByZeroException();

            return new Rational(
                dividend.numerator * divisor.denominator,
                dividend.denominator * divisor.numerator);
        }

        /// <summary>
        /// Negates a specified Rational value.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The result of the value parameter multiplied by negative one.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// value is a null reference.</exception>
        public static Rational Negate(Rational value)
        {
            return -value;
        }

        /// <summary>
        /// Negates a specified Rational value.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The result of the value parameter multiplied by negative one.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// value is a null reference.</exception>
        public static Rational operator -(Rational value)
        {
            if (object.ReferenceEquals(value, null))
                throw new ArgumentNullException(nameof(value));

            return new Rational(-value.numerator, value.denominator, false);
        }

        /// <summary>
        /// Returns the smaller of two Rational values.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>The left or right parameter, whichever is smaller.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// left or right are null references.</exception>
        public static Rational Min(Rational left, Rational right)
        {
            if (object.ReferenceEquals(left, null))
                throw new ArgumentNullException(nameof(left));
            if (object.ReferenceEquals(right, null))
                throw new ArgumentNullException(nameof(right));

            return left < right ? left : right;
        }

        /// <summary>
        /// Returns the larger of two Rational values.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>The left or right parameter, whichever is larger.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// left or right are null references.</exception>
        public static Rational Max(Rational left, Rational right)
        {
            if (object.ReferenceEquals(left, null))
                throw new ArgumentNullException(nameof(left));
            if (object.ReferenceEquals(right, null))
                throw new ArgumentNullException(nameof(right));

            return left > right ? left : right;
        }

        #endregion


        #region Equality

        /// <summary>
        /// Determines whether the specified System.Object object is equal to the
        /// current Rational object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the obj parameter is a Rational object or a type capable of
        /// implicit conversion to a Rational value, and its value is equal to the value of
        /// the current Rational object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Rational);
        }

        /// <summary>
        /// Determines whether the specified Rational object is equal to the
        /// current Rational object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>true if the value of the obj parameter is equal to the value
        /// of the current Rational object; otherwise, false.</returns>
        public bool Equals(Rational other)
        {
            return 0 == CompareRationals(this, other);
        }

        /// <summary>
        /// Serves as a hash function for the Rational type.
        /// </summary>
        /// <returns>A hash code for the current Rational object.</returns>
        public override int GetHashCode()
        {
            return (numerator.GetHashCode() << 16) ^ (denominator.GetHashCode() & 0xFFFF);
        }

        #endregion


        #region Comparison

        /// <summary>
        /// Indicates whether a Rational object is greater than another Rational object.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if left is greater than right; otherwise, false.</returns>
        public static bool operator >(Rational left, Rational right)
        {
            return 1 == CompareRationals(left, right);
        }

        /// <summary>
        /// Indicates whether a Rational object is less than or equal to
        /// another Rational object.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if left is less than or equal to right; otherwise, false.</returns>
        public static bool operator <=(Rational left, Rational right)
        {
            return 1 != CompareRationals(left, right);
        }

        /// <summary>
        /// Indicates whether a Rational object is less than another Rational object.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if left is less than right; otherwise, false.</returns>
        public static bool operator <(Rational left, Rational right)
        {
            return -1 == CompareRationals(left, right);
        }

        /// <summary>
        /// Indicates whether a Rational object is greater than or equal to
        /// another Rational object.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if left is greater than or equal to right;
        /// otherwise, false.</returns>
        public static bool operator >=(Rational left, Rational right)
        {
            return -1 != CompareRationals(left, right);
        }

        /// <summary>
        /// Indicates whether the values of two Rational objects are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the left and right parameters have the same value;
        /// otherwise, false.</returns>
        public static bool operator ==(Rational left, Rational right)
        {
            return 0 == CompareRationals(left, right);
        }

        /// <summary>
        /// Indicates whether the values of two Rational objects are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>false if the left and right parameters have the same value;
        /// otherwise, true.</returns>
        public static bool operator !=(Rational left, Rational right)
        {
            return 0 != CompareRationals(left, right);
        }

        /// <summary>
        /// Compares the current Rational object to a second Rational object
        /// and returns an integer that indicates whether the value of this object is
        /// less than, equal to, or greater than the value of the specified object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>Zero, if the two objects have the same value.
        /// Negative one, if the value of this object is less than the value of other.
        /// One, if the value of this object is greater than the value of other.</returns>
        public int CompareTo(Rational other)
        {
            return CompareRationals(this, other);
        }

        /// <summary>
        /// Compares the current Rational object to a System.Object object
        /// and returns an integer that indicates whether the value of this object is
        /// less than, equal to, or greater than the value of the specified object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>Zero, if the two objects have the same value.
        /// Negative one, if the value of this object is less than the value of other.
        /// One, if the value of this object is greater than the value of other.</returns>
        /// <exception cref="System.ArgumentException">obj is not a Rational.</exception>
        public int CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null))
                return 1;

            var castObj = obj as Rational;

            if (object.ReferenceEquals(castObj, null))
                throw new ArgumentException("Object must be of type Rational.", nameof(obj));

            return CompareRationals(this, castObj);
        }

        static int CompareRationals(Rational a, Rational b)
        {
            if (object.ReferenceEquals(a, null))
            {
                if (object.ReferenceEquals(b, null))
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (object.ReferenceEquals(b, null))
                    return 1;
                else
                {
                    if (a.denominator == b.denominator)
                        return a.numerator.CompareTo(b.numerator);
                    else
                        return (a.numerator * b.denominator)
                            .CompareTo(a.denominator * b.numerator);
                }
            }
        }

        #endregion


        #region Text Serialization

        /// <summary>
        /// Converts the string representation of a numeric value to its
        /// equivalent Rational object.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A Rational object with the current value.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// value is a null reference.</exception>
        /// <exception cref="System.FormatException">
        /// Input string is not in a correct format.</exception>
        public static Rational Parse(string value)
        {
            if (null == value)
                throw new ArgumentNullException(nameof(value));

            value = value.Trim();
            var sign = BigInteger.One;
            if (value.StartsWith("-", StringComparison.Ordinal))
            {
                sign = BigInteger.MinusOne;
                value = value.Substring(1);
            }

            if (value.Contains('.'))
            {
                string[] tokens = value.Split('.');

                if (tokens.Length == 2)
                {
                    var whole = ParseBigInteger(tokens[0], BigInteger.Zero);
                    var fraction = ParseBigInteger(tokens[1], BigInteger.Zero);

                    var denominator = BigInteger.Pow(10, tokens[1].Length);
                    var numerator = whole * denominator + fraction;

                    return new Rational(sign * numerator, denominator);
                }
                else
                    throw new FormatException("Input string was not in a correct format.");
            }
            else
            {
                var tokens = value.Split('/');

                if (tokens.Length == 1)
                    return new Rational(sign * ParseBigInteger(tokens[0], BigInteger.Zero));
                else if (tokens.Length == 2)
                    return new Rational(
                        sign * ParseBigInteger(tokens[0], BigInteger.Zero),
                        ParseBigInteger(tokens[1], BigInteger.One));
                else
                    throw new FormatException("Input string was not in a correct format.");
            }
        }

        static BigInteger ParseBigInteger(string value, BigInteger minValue)
        {
            if (false == System.Text.RegularExpressions.Regex.IsMatch(value, "^[0-9]+$"))
                throw new FormatException("Input string was not in a correct format.");

            var result = BigInteger.Parse(value, CultureInfo.InvariantCulture);

            if (result < minValue)
                throw new FormatException("Input string was not in a correct format.");

            return result;
        }

        /// <summary>
        /// Converts the numeric value of the current Rational object to its equivalent
        /// string representation as a mixed fraction.
        /// </summary>
        /// <returns>The string representation of the current Rational value.</returns>
        public override string ToString()
        {
            if (denominator.IsOne)
                return numerator.ToString(CultureInfo.InvariantCulture);
            else
                return string.Format(CultureInfo.InvariantCulture,
                    "{0}/{1}", numerator, denominator);
        }

        #endregion


        #region Binary Serialization

        /// <summary>
        /// Reads the binary representation of a Rational object from a stream.
        /// </summary>
        /// <param name="input">
        /// The stream from which to read the binary representation.</param>
        /// <returns>The Rational value read.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// input is a null reference.</exception>
        /// <exception cref="System.FormatException">
        /// data in intput stream could not be converted to a Rational object.</exception>
        public static Rational Deserialize(Stream input)
        {
            if (null == input)
                throw new ArgumentNullException(nameof(input));

            var reader = new BinaryReader(input, System.Text.Encoding.ASCII, true);

            var numeratorByteCount = reader.ReadInt32();
            var denominatorByteCount = reader.ReadInt32();

            if (1 > numeratorByteCount)
                throw new FormatException("Numerator byte count corrupt.");
            if (1 > denominatorByteCount)
                throw new FormatException("Denominator byte count corrupt.");

            var numerator = new BigInteger(reader.ReadBytes(numeratorByteCount));
            var denominator = new BigInteger(reader.ReadBytes(denominatorByteCount));

            if (denominator < 1)
                throw new FormatException("Denominator corrupt.");

            return new Rational(numerator, denominator);
        }

        /// <summary>
        /// Writes a binary representation of the current Rational value to a stream.
        /// </summary>
        /// <param name="output">
        /// The stream to which to write the binary representation.</param>
        /// <exception cref="System.ArgumentNullException">
        /// output is a null reference.</exception>
        /// <exception cref="System.NotSupportedException">
        /// numerator or denominator are more than System.Int32.MaxValue
        /// bytes in size.</exception>
        public void Serialize(Stream output)
        {
            if (null == output)
                throw new ArgumentNullException(nameof(output));

            var writer = new BinaryWriter(output, System.Text.Encoding.ASCII, true);

            var numeratorBytes = numerator.ToByteArray();
            var denominatorBytes = denominator.ToByteArray();

            ThrowIfTooBig(numeratorBytes, denominatorBytes);

            writer.Write((uint)numeratorBytes.LongLength);
            writer.Write((uint)denominatorBytes.LongLength);
            writer.Write(numeratorBytes, 0, numeratorBytes.Length);
            writer.Write(denominatorBytes, 0, denominatorBytes.Length);
        }

        [ExcludeFromCodeCoverage(Justification =
            "Not practically possible to make a BigInteger this big")]
        static void ThrowIfTooBig(byte[] numeratorBytes, byte[] denominatorBytes)
        {
            if (numeratorBytes.LongLength > Int32.MaxValue ||
                            denominatorBytes.LongLength > Int32.MaxValue)
                throw new OverflowException("Value too large to serialize");
        }

        #endregion
    }
}
