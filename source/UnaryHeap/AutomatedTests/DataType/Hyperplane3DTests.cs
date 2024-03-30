﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnaryHeap.DataType;

namespace AutomatedTests.DataType
{
    /// <summary>
    /// Represents a hyperplane in two-dimensional space (i.e. a plane).
    /// </summary>
    class Hyperplane3D
    {
        /// <summary>
        /// The plane normal's X coefficient.
        /// </summary>
        public Rational A { get; private set; }
        /// <summary>
        /// The plane normal's Y coefficient.
        /// </summary>
        public Rational B { get; private set; }
        /// <summary>
        /// The plane normal's Z coefficient.
        /// </summary>
        public Rational C { get; private set; }
        /// <summary>
        /// The plane normal's constant term.
        /// </summary>
        public Rational D { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Hyperplane3D class.
        /// </summary>
        /// <param name="a">The X coefficient of the plane normal.</param>
        /// <param name="b">The Y coefficient of the plane normal.</param>
        /// <param name="c">The Z coefficient of the plane normal.</param>
        /// <param name="d">The constant term of the plane normal.</param>
        /// <exception cref="System.ArgumentNullException">a, b, c or d are null.</exception>
        /// <exception cref="System.ArgumentException">
        /// a, b and c are all equal to zero.
        /// </exception>
        public Hyperplane3D(Rational a, Rational b, Rational c, Rational d)
        {
            if (null == a)
                throw new ArgumentNullException(nameof(a));
            if (null == b)
                throw new ArgumentNullException(nameof(b));
            if (null == c)
                throw new ArgumentNullException(nameof(c));
            if (null == d)
                throw new ArgumentNullException(nameof(d));
            if (0 == a && 0 == b && 0 == c)
                throw new ArgumentException("Hyperplane normal has zero length.");

            A = a;
            B = b;
            C = c;
            D = d;

            NormalizeCoefficients();
        }

        /// <summary>
        /// Determines where the given point lies in relation to this Hyperplane3D.
        /// </summary>
        /// <param name="p">The point to classify.</param>
        /// <returns>A positive value, if p is in the front halfspace of this Hyperplane3D.
        /// A negative value, if p is in the back halfspace of this Hyperplane3D.
        /// Zero, if p is incident to this Hyperplane3D.</returns>
        /// <exception cref="System.ArgumentNullException">p is null.</exception>
        public int DetermineHalfspaceOf(Point3D p)
        {
            if (null == p)
                throw new ArgumentNullException(nameof(p));

            return (A * p.X + B * p.Y + C * p.Z + D).Sign;
        }

        void NormalizeCoefficients()
        {
            // --- 'Normalize' the coefficients to optimize equality testing
            // --- to simply be direct comparison of values.

            var e = Rational.Max(A.AbsoluteValue, Rational.Max(B.AbsoluteValue, C.AbsoluteValue));
            A /= e;
            B /= e;
            C /= e;
            D /= e;
        }


        /// <summary>
        /// Gets a copy of the current Hyperplane3D, with the front and half
        /// halfspaces swapped.
        /// </summary>
        public Hyperplane3D Coplane
        {
            get { return new Hyperplane3D(-A, -B, -C, -D); }
        }

        /// <summary>
        /// Indicates whether the current Hyperplane3D is equal to another Hyperplane3D.
        /// </summary>
        /// <param name="other">The Hyperplane to compare with this Hyperplane.</param>
        /// <returns>true if the current Hyperplane is equal to the other parameter;
        /// otherwise, false.</returns>
        public bool Equals(Hyperplane3D other)
        {
            if (null == other)
                return false;

            return
                this.A == other.A &&
                this.B == other.B &&
                this.C == other.C &&
                this.D == other.D;
        }


        /// <summary>
        /// Indicates whether the current Hyperplane3D is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare with this Hyperplane.</param>
        /// <returns>true if the current Hyperplane is equal to the obj parameter;
        /// otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Hyperplane3D);
        }

        /// <summary>
        /// Serves as a hash function for the Hyperplane3D type.
        /// </summary>
        /// <returns>A hash code for the current Hyperplane3D object.</returns>
        public override int GetHashCode()
        {
            return new Point3D(A, B, C).GetHashCode() ^ D.GetHashCode();
        }

        public List<Point3D> MakePolytope(Rational size)
        {
            // TODO: this method will return incorrect windings for some planes;
            // need to reverse the order sometimes
            // Comprehensive unit tests will ferret it out
            if (A != 0)
            {
                return new List<Point3D>()
                {
                    SolveForX(size, size),
                    SolveForX(size, -size),
                    SolveForX(-size, -size),
                    SolveForX(-size, size),
                };
            }
            else if (B != 0)
            {
                return new List<Point3D>()
                {
                    SolveForY(size, size),
                    SolveForY(size, -size),
                    SolveForY(-size, -size),
                    SolveForY(-size, size),
                };
            }
            else // Constructor affirms that C != 0
            {
                return new List<Point3D>()
                {
                    SolveForZ(size, size),
                    SolveForZ(size, -size),
                    SolveForZ(-size, -size),
                    SolveForZ(-size, size),
                };
            }
        }

        Point3D SolveForX(Rational y, Rational z)
        {
            return new Point3D(-(B * y + C * z + D) / A, y, z);
        }

        Point3D SolveForY(Rational x, Rational z)
        {
            return new Point3D(x, -(A * x + C * z + D) / B, z);
        }

        Point3D SolveForZ(Rational x, Rational y)
        {
            return new Point3D(x, y, -(A * x + B * y + D) / C);
        }
    }

    [TestFixture]
    public class Hyperplane3DTests
    {
    }
}
