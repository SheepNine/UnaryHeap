﻿using System;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a two-dimensional matrix.
    /// </summary>
    public class Matrix2D
    {
        #region Static Constructors

        /// <summary>
        /// Returns the two-dimensional identity matrix.
        /// </summary>
        public static readonly Matrix2D Identity = new Matrix2D(1, 0, 0, 1);

        /// <summary>
        /// Returns the two-dimensional matrix that inverts the Y-coordinate of input points.
        /// </summary>
        public static readonly Matrix2D XReflection = new Matrix2D(1, 0, 0, -1);

        /// <summary>
        /// Returns the two-dimensional matrix that inverts the X-coordinate of input points.
        /// </summary>
        public static readonly Matrix2D YReflection = new Matrix2D(-1, 0, 0, 1);

        /// <summary>
        /// Produces a two-dimensional matrix that shears the X-coordinate of input points.
        /// </summary>
        /// <param name="factor">
        /// The multiplicative factor applied to the point's Y-coordinate.</param>
        /// <returns>A two-dimensional matrix that shears the X-coordinate
        /// of input points.</returns>
        /// <exception cref="System.ArgumentNullException">factor is null.</exception>
        public static Matrix2D XShear(Rational factor)
        {
            ArgumentNullException.ThrowIfNull(factor);

            return new Matrix2D(1, factor, 0, 1);
        }

        /// <summary>
        /// Produces a two-dimensional matrix that shears the Y-coordinate of input points.
        /// </summary>
        /// <param name="factor">
        /// The multiplicative factor applied to the point's X-coordinate.</param>
        /// <returns>A two-dimensional matrix that shears the Y-coordinate
        /// of input points.</returns>
        /// <exception cref="System.ArgumentNullException">factor is null.</exception>
        public static Matrix2D YShear(Rational factor)
        {
            ArgumentNullException.ThrowIfNull(factor);

            return new Matrix2D(1, 0, factor, 1);
        }

        /// <summary>
        /// Produces a two-dimensional matrix that scales the coordinates of input points
        /// by a constant factor.
        /// </summary>
        /// <param name="factor">The constant factor applied to the
        /// coordinates of input points.</param>
        /// <returns>A two-dimensional matrix that scales the coordinates of input points
        /// by a constant factor.</returns>
        public static Matrix2D Scale(Rational factor)
        {
            ArgumentNullException.ThrowIfNull(factor);

            return new Matrix2D(factor, 0, 0, factor);
        }

        #endregion


        #region Member Variables

        Rational[][] rows;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Matrix2D class.
        /// </summary>
        /// <param name="elem00">The coefficient in the first row
        /// and first column of the matrix.</param>
        /// <param name="elem01">The coefficient in the first row
        /// and second column of the matrix.</param>
        /// <param name="elem10">The coefficient in the second row
        /// and first column of the matrix.</param>
        /// <param name="elem11">The coefficient in the second row
        /// and second column of the matrix.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Any of elem00, elem01, elem10 or elem11 are null.</exception>
        public Matrix2D(Rational elem00, Rational elem01, Rational elem10, Rational elem11)
        {
            ArgumentNullException.ThrowIfNull(elem00);
            ArgumentNullException.ThrowIfNull(elem01);
            ArgumentNullException.ThrowIfNull(elem10);
            ArgumentNullException.ThrowIfNull(elem11);

            rows = new[]
            {
                new[] { elem00, elem01 },
                new[] { elem10, elem11 }
            };
        }

        Matrix2D(Rational[][] rows)
        {
            this.rows = rows;
        }

        #endregion


        #region Operator overloads

        /// <summary>
        /// Computes the product of two matrices.
        /// </summary>
        /// <param name="left">The 'left' matrix of the computation.</param>
        /// <param name="right">The 'right' matrix of the computation.</param>
        /// <returns>A Matrix2D whose coefficients are computed by taking the dot products
        /// of rows from left and columns from right.</returns>
        /// <exception cref="System.ArgumentNullException">left or right are null.</exception>
        public static Matrix2D operator *(Matrix2D left, Matrix2D right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            return new Matrix2D(Matrix.Multiply(2, left.rows, right.rows));
        }

        /// <summary>
        /// Computes the product of two matrices.
        /// </summary>
        /// <param name="left">The 'left' matrix of the computation.</param>
        /// <param name="right">The 'right' matrix of the computation.</param>
        /// <returns>A Matrix2D whose coefficients are computed by taking the dot products
        /// of rows from left and columns from right.</returns>
        /// <exception cref="System.ArgumentNullException">left or right are null.</exception>
        public static Matrix2D Multiply(Matrix2D left, Matrix2D right)
        {
            return left * right;
        }

        /// <summary>
        /// Computes the scalar multiple of a matrix.
        /// </summary>
        /// <param name="c">The scalar multiple.</param>
        /// <param name="m">The matrix.</param>
        /// <returns>A Matrix2D whose coefficients are the product of the elemtns of
        /// m and the scalar c.</returns>
        /// <exception cref="System.ArgumentNullException">c or m are null.</exception>
        public static Matrix2D operator *(Rational c, Matrix2D m)
        {
            ArgumentNullException.ThrowIfNull(c);
            ArgumentNullException.ThrowIfNull(m);

            return new Matrix2D(
                c * m.rows[0][0], c * m.rows[0][1],
                c * m.rows[1][0], c * m.rows[1][1]
            );
        }

        /// <summary>
        /// Computes the scalar multiple of a matrix.
        /// </summary>
        /// <param name="c">The scalar multiple.</param>
        /// <param name="m">The matrix.</param>
        /// <returns>A Matrix2D whose coefficients are the product of the elemtns of
        /// m and the scalar c.</returns>
        /// <exception cref="System.ArgumentNullException">c or m are null.</exception>
        public static Matrix2D Scale(Rational c, Matrix2D m)
        {
            return c * m;
        }

        /// <summary>
        /// Computes the linear transformation of a point.
        /// </summary>
        /// <param name="m">The matrix corresponding to the transformation.</param>
        /// <param name="p">The point to transform.</param>
        /// <returns>A Point2D whose coefficients are the dot product of p and
        /// rows of m.</returns>
        /// <exception cref="System.ArgumentNullException">m or p are null.</exception>
        public static Point2D operator *(Matrix2D m, Point2D p)
        {
            ArgumentNullException.ThrowIfNull(m);
            ArgumentNullException.ThrowIfNull(p);

            return new Point2D(RowMultiply(m.rows[0], p), RowMultiply(m.rows[1], p));
        }

        /// <summary>
        /// Computes the linear transformation of a point.
        /// </summary>
        /// <param name="m">The matrix corresponding to the transformation.</param>
        /// <param name="p">The point to transform.</param>
        /// <returns>A Point2D whose coefficients are the dot product of p and
        /// rows of m.</returns>
        /// <exception cref="System.ArgumentNullException">m or p are null.</exception>
        public static Point2D Transform(Matrix2D m, Point2D p)
        {
            return m * p;
        }

        static Rational RowMultiply(Rational[] row, Point2D p)
        {
            return p.X * row[0] + p.Y * row[1];
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets a coefficient in the matrix.
        /// </summary>
        /// <param name="row">The row of the coefficient to retrieve.</param>
        /// <param name="col">The column of the coefficient to retrieve.</param>
        /// <returns>The coefficient at the specified row and column.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">row or col are less than zero
        /// or greater than one.</exception>
        public Rational this[int row, int col]
        {
            get
            {
                if (0 > row || 1 < row)
                    throw new ArgumentOutOfRangeException(nameof(row));
                if (0 > col || 1 < col)
                    throw new ArgumentOutOfRangeException(nameof(col));

                return rows[row][col];
            }
        }


        #endregion


        #region Public Methods

        /// <summary>
        /// Computes the inverse of this Matrix2D.
        /// </summary>
        /// <returns>The Matrix2D that, when multiplied by this Matrix2D, yields the
        /// Matrix2D.Identity.</returns>
        /// <exception cref="InvalidOperationException">This matrix is singular
        /// (i.e. its determinant is zero).</exception>
        public Matrix2D ComputeInverse()
        {
            return new Matrix2D(Matrix.Invert(2, rows));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Matrix.StringFormat(2, rows);
        }

        #endregion
    }
}
