using System;
using System.IO;
using System.Numerics;

namespace UnaryHeap.Utilities
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
        /// Initializes a new instance of the UnaryHeap.Utilities.Rational class from the given integeral value.
        /// </summary>
        /// <param name="integer">The value to initialize the new instance to.</param>
        public Rational(BigInteger integer)
            : this(integer, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnaryHeap.Utilities.Rational class from the given integral numerator and denominator.
        /// </summary>
        /// <param name="numerator">The numerator of the new instance.</param>
        /// <param name="denominator">The denominator of the new instance.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Denominator is zero.</exception>
        public Rational(BigInteger numerator, BigInteger denominator)
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
                throw new ArgumentOutOfRangeException("denominator", "Denominator cannot be zero.");
            }

            var gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);

            if (false == gcd.IsOne)
            {
                this.numerator = this.numerator / gcd;
                this.denominator = this.denominator / gcd;
            }
        }

        #region Conversion Operators

        /// <summary>
        /// Defines an explicit conversion of a System.Int32 object to a UnaryHeap.Utilities.Rational value.
        /// </summary>
        /// <param name="integer">The value to convert to a UnaryHeap.Utilities.Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(int integer)
        {
            return new Rational(integer);
        }

        /// <summary>
        /// Defines an explicit conversion of a System.UInt32 object to a UnaryHeap.Utilities.Rational value.
        /// </summary>
        /// <param name="integer">The value to convert to a UnaryHeap.Utilities.Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(uint integer)
        {
            return new Rational(integer);
        }

        /// <summary>
        /// Defines an explicit conversion of a System.Int64 object to a UnaryHeap.Utilities.Rational value.
        /// </summary>
        /// <param name="integer">The value to convert to a UnaryHeap.Utilities.Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(long integer)
        {
            return new Rational(integer);
        }

        /// <summary>
        /// Defines an explicit conversion of a System.UInt64 object to a UnaryHeap.Utilities.Rational value.
        /// </summary>
        /// <param name="integer">The value to convert to a UnaryHeap.Utilities.Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(ulong integer)
        {
            return new Rational(integer);
        }

        /// <summary>
        /// Defines an explicit conversion of a System.Int16 object to a UnaryHeap.Utilities.Rational value.
        /// </summary>
        /// <param name="integer">The value to convert to a UnaryHeap.Utilities.Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(short integer)
        {
            return new Rational(integer);
        }

        /// <summary>
        /// Defines an explicit conversion of a System.UInt16 object to a UnaryHeap.Utilities.Rational value.
        /// </summary>
        /// <param name="integer">The value to convert to a UnaryHeap.Utilities.Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(ushort integer)
        {
            return new Rational(integer);
        }

        /// <summary>
        /// Defines an explicit conversion of a System.SByte object to a UnaryHeap.Utilities.Rational value.
        /// </summary>
        /// <param name="integer">The value to convert to a UnaryHeap.Utilities.Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(sbyte integer)
        {
            return new Rational(integer);
        }

        /// <summary>
        /// Defines an explicit conversion of a System.Byte object to a UnaryHeap.Utilities.Rational value.
        /// </summary>
        /// <param name="integer">The value to convert to a UnaryHeap.Utilities.Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(byte integer)
        {
            return new Rational(integer);
        }

        /// <summary>
        /// Defines an explicit conversion of a System.Numerics.BigInteger object to a UnaryHeap.Utilities.Rational value.
        /// </summary>
        /// <param name="integer">The value to convert to a UnaryHeap.Utilities.Rational.</param>
        /// <returns> An object that contains the value of the value parameter.</returns>
        public static implicit operator Rational(BigInteger integer)
        {
            return new Rational(integer);
        }

        #endregion

        #endregion


        #region Properties

        /// <summary>
        /// Gets the numerator of the current UnaryHeap.Utilities.Rational object.
        /// </summary>
        public BigInteger Numerator
        {
            get { return numerator; }
        }

        /// <summary>
        /// Gets the denominator of the current UnaryHeap.Utilities.Rational object.
        /// </summary>
        public BigInteger Denominator
        {
            get { return denominator; }
        }

        /// <summary>
        /// Gets a number that indicates the sign (negative, positive, or zero) of the current UnaryHeap.Utilities.Rational object.
        /// </summary>
        public int Sign
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Indicates whether the current UnaryHeap.Utilities.Rational object is less than zero.
        /// </summary>
        public bool IsNegative
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Indicates whether the current UnaryHeap.Utilities.Rational object is zero.
        /// </summary>
        public bool IsZero
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Indicates whether the current UnaryHeap.Utilities.Rational object is greater than zero.
        /// </summary>
        public bool IsPositive
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the absolute value of the current UnaryHeap.Utilities.Rational object.
        /// </summary>
        public Rational AbsoluteValue
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the largest whole number less than or equal to the current UnaryHeap.Utilities.Rational object.
        /// </summary>
        public Rational Floor
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the smallest whole number greater than or equal to the current UnaryHeap.Utilities.Rational object.
        /// </summary>
        public Rational Ceiling
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the value of the current UnaryHeap.Utilities.Rational object raised to the second power.
        /// </summary>
        public Rational Squared
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the closest whole number to the current UnaryHeap.Utilities.Rational object. If the current object is halfway between
        /// two whole numbers, the even number is returned.
        /// </summary>
        public Rational Rounded
        {
            get { throw new NotImplementedException(); }
        }

        #endregion


        #region Operators

        /// <summary>
        /// Adds the values of two specified UnaryHeap.Utilities.Rational objects.
        /// </summary>
        /// <param name="left">The first value to add.</param>
        /// <param name="right">The second value to add.</param>
        /// <returns>The sum of left and right.</returns>
        public static Rational operator +(Rational left, Rational right)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Subtracts a UnaryHeap.Utilities.Rational value from another UnaryHeap.Utilities.Rational value.
        /// </summary>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend).</param>
        /// <returns>The result of subtracting right from left.</returns>
        public static Rational operator -(Rational left, Rational right)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Multiplies two specified UnaryHeap.Utilities.Rational values.
        /// </summary>
        /// <param name="left">The first value to multiply.</param>
        /// <param name="right">The second value to multiply.</param>
        /// <returns>The product of left and right.</returns>
        public static Rational operator *(Rational left, Rational right)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Divides a specified UnaryHeap.Utilities.Rational value by another specified UnaryHeap.Utilities.Rational value by using integer division.
        /// </summary>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <returns>The integral result of the division.</returns>
        /// <exception cref="System.DivideByZeroException">Divisor is zero.</exception>
        public static Rational operator /(Rational dividend, Rational divisor)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Negates a specified UnaryHeap.Utilities.Rational value.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The result of the value parameter multiplied by negative one.</returns>
        public static Rational operator -(Rational value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the smaller of two UnaryHeap.Utilities.Rational values.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>The left or right parameter, whichever is smaller.</returns>
        public static Rational Min(Rational left, Rational right)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the larger of two UnaryHeap.Utilities.Rational values.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>The left or right parameter, whichever is larger.</returns>
        public static Rational Max(Rational left, Rational right)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Equality

        /// <summary>
        /// Determines whether the specified System.Object object is equal to the current UnaryHeap.Utilities.Rational object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the obj parameter is a UnaryHeap.Utilities.Rational object or a type capable of implicit
        /// conversion to a UnaryHeap.Utilities.Rational value, and its value is equal to the value of the current
        /// UnaryHeap.Utilities.Rational object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Rational);
        }

        /// <summary>
        /// Determines whether the specified UnaryHeap.Utilities.Rational object is equal to the current UnaryHeap.Utilities.Rational object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the value of the obj parameter is equal to the value of the current UnaryHeap.Utilities.Rational object; otherwise, false.</returns>
        public bool Equals(Rational obj)
        {
            return 0 == CompareRationals(this, obj);
        }

        /// <summary>
        /// Serves as a hash function for the UnaryHeap.Utilities.Rational type.
        /// </summary>
        /// <returns>A hash code for the current UnaryHeap.Utilities.Rational object.</returns>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Comparison

        /// <summary>
        /// Indicates whether a UnaryHeap.Utilities.Rational object is greater than another UnaryHeap.Utilities.Rational object.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if left is greater than right; otherwise, false.</returns>
        public static bool operator >(Rational left, Rational right)
        {
            return 1 == CompareRationals(left, right);
        }

        /// <summary>
        /// Indicates whether a UnaryHeap.Utilities.Rational object is less than or equal to another UnaryHeap.Utilities.Rational object.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if left is less than or equal to right; otherwise, false.</returns>
        public static bool operator <=(Rational left, Rational right)
        {
            return 1 != CompareRationals(left, right);
        }

        /// <summary>
        /// Indicates whether a UnaryHeap.Utilities.Rational object is less than another UnaryHeap.Utilities.Rational object.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if left is less than right; otherwise, false.</returns>
        public static bool operator <(Rational left, Rational right)
        {
            return -1 == CompareRationals(left, right);
        }

        /// <summary>
        /// Indicates whether a UnaryHeap.Utilities.Rational object is greater than or equal to another UnaryHeap.Utilities.Rational object.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if left is greater than or equal to right; otherwise, false.</returns>
        public static bool operator >=(Rational left, Rational right)
        {
            return -1 != CompareRationals(left, right);
        }

        /// <summary>
        /// Indicates whether the values of two UnaryHeap.Utilities.Rational objects are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the left and right parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(Rational left, Rational right)
        {
            return 0 == CompareRationals(left, right);
        }

        /// <summary>
        /// Indicates whether the values of two UnaryHeap.Utilities.Rational objects are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>false if the left and right parameters have the same value; otherwise, true.</returns>
        public static bool operator !=(Rational left, Rational right)
        {
            return 0 != CompareRationals(left, right);
        }

        /// <summary>
        /// Compares the current UnaryHeap.Utilities.Rational object to a second UnaryHeap.Utilities.Rational object
        /// and returns an integer that indicates whether the value of this object is less than, equal to,
        /// or greater than the value of the specified object.
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
        /// Compares the current UnaryHeap.Utilities.Rational object to a System.Object object
        /// and returns an integer that indicates whether the value of this object is less than, equal to,
        /// or greater than the value of the specified object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>Zero, if the two objects have the same value.
        /// Negative one, if the value of this object is less than the value of other.
        /// One, if the value of this object is greater than the value of other.</returns>
        /// <exception cref="System.ArgumentException">obj is not a UnaryHeap.Utilities.Rational.</exception>
        public int CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null))
                return 1;
            if (false == (obj is Rational))
                throw new ArgumentException("Object must be of type UnaryHeap.Utilities.Rational.", "obj");

            return CompareRationals(this, obj as Rational);
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
                        return (a.numerator * b.denominator).CompareTo(a.denominator * b.numerator);
                }
            }
        }

        #endregion


        #region Text Serialization

        /// <summary>
        /// Converts the string representation of a numeric value to its equivalent UnaryHeap.Utilities.Rational object.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A UnaryHeap.Utilities.Rational object with the current value.</returns>
        public static Rational Parse(string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the numeric value of the current UnaryHeap.Utilities.Rational object to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the current UnaryHeap.Utilities.Rational value.</returns>
        public override string ToString()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Binary Serialization

        /// <summary>
        /// Reads the binary representation of a UnaryHeap.Utilities.Rational object from a stream.
        /// </summary>
        /// <param name="source">The stream from which to read the binary representation.</param>
        /// <returns>The UnaryHeap.Utilities.Rational value read.</returns>
        public static Rational Deserialize(Stream source)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a binary representation of the current UnaryHeap.Utilities.Rational value to a stream.
        /// </summary>
        /// <param name="destination">The stream to which to write the binary representation.</param>
        public void Serialize(Stream destination)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
