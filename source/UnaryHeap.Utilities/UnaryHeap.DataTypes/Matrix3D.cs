using System;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a three-dimensional matrix.
    /// </summary>
    public class Matrix3D
    {
        /// <summary>
        /// Returns the two-dimensional identity matrix.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "Matrix3D is immutable.")]
        public static readonly Matrix3D Identity = new Matrix3D(1, 0, 0, 0, 1, 0, 0, 0, 1);

        Rational[][] rows;

        /// <summary>
        /// Initializes a new instance of the Matrix3D class
        /// </summary>
        /// <param name="elem00">The coefficient in the first row and first column</param>
        /// <param name="elem01">The coefficient in the first row and second column</param>
        /// <param name="elem02">The coefficient in the first row and third column</param>
        /// <param name="elem10">The coefficient in the second row and first column</param>
        /// <param name="elem11">The coefficient in the second row and second column</param>
        /// <param name="elem12">The coefficient in the second row and third column</param>
        /// <param name="elem20">The coefficient in the third row and first column</param>
        /// <param name="elem21">The coefficient in the third row and second column</param>
        /// <param name="elem22">The coefficient in the third row and third column</param>
        /// <exception cref="System.ArgumentNullException">
        /// Any of the parameters are null.</exception>
        public Matrix3D(
            Rational elem00, Rational elem01, Rational elem02,
            Rational elem10, Rational elem11, Rational elem12,
            Rational elem20, Rational elem21, Rational elem22)
        {
            if (null == elem00)
                throw new ArgumentNullException("elem00");
            if (null == elem01)
                throw new ArgumentNullException("elem01");
            if (null == elem02)
                throw new ArgumentNullException("elem02");
            if (null == elem10)
                throw new ArgumentNullException("elem10");
            if (null == elem11)
                throw new ArgumentNullException("elem11");
            if (null == elem12)
                throw new ArgumentNullException("elem12");
            if (null == elem20)
                throw new ArgumentNullException("elem20");
            if (null == elem21)
                throw new ArgumentNullException("elem21");
            if (null == elem22)
                throw new ArgumentNullException("elem22");

            rows = new[]
            {
                new[] { elem00, elem01, elem02 },
                new[] { elem10, elem11, elem12 },
                new[] { elem20, elem21, elem22 },
            };
        }

        Matrix3D(Rational[][] rows)
        {
            this.rows = rows;
        }

        /// <summary>
        /// Computes the product of two matrices.
        /// </summary>
        /// <param name="left">The 'left' matrix of the computation.</param>
        /// <param name="right">The 'right' matrix of the computation.</param>
        /// <returns>A Matrix3D whose coefficients are computed by taking the dot products
        /// of rows from left and columns from right.</returns>
        /// <exception cref="System.ArgumentNullException">left or right are null.</exception>
        public static Matrix3D operator *(Matrix3D left, Matrix3D right)
        {
            if (null == left)
                throw new ArgumentNullException("left");
            if (null == right)
                throw new ArgumentNullException("right");

            return new Matrix3D(Matrix.Multiply(3, left.rows, right.rows));
        }

        /// <summary>
        /// Computes the product of two matrices.
        /// </summary>
        /// <param name="left">The 'left' matrix of the computation.</param>
        /// <param name="right">The 'right' matrix of the computation.</param>
        /// <returns>A Matrix3D whose coefficients are computed by taking the dot products
        /// of rows from left and columns from right.</returns>
        /// <exception cref="System.ArgumentNullException">left or right are null.</exception>
        public static Matrix3D Multiply (Matrix3D left, Matrix3D right)
        {
            return left * right;
        }

        /// <summary>
        /// Computes the linear transformation of a point.
        /// </summary>
        /// <param name="m">The matrix corresponding to the transformation.</param>
        /// <param name="p">The point to transform.</param>
        /// <returns>A Point3D whose coefficients are the dot product of p and
        /// rows of m.</returns>
        /// <exception cref="System.ArgumentNullException">m or p are null.</exception>
        public static Point3D operator *(Matrix3D m, Point3D p)
        {
            if (null == m)
                throw new ArgumentNullException("m");
            if (null == p)
                throw new ArgumentNullException("p");

            return new Point3D(
                RowMultiply(m.rows[0], p),
                RowMultiply(m.rows[1], p),
                RowMultiply(m.rows[2], p)
            );
        }

        static Rational RowMultiply(Rational[] row, Point3D p)
        {
            return p.X * row[0] + p.Y * row[1] + p.Z * row[2];
        }

        /// <summary>
        /// Computes the linear transformation of a point.
        /// </summary>
        /// <param name="m">The matrix corresponding to the transformation.</param>
        /// <param name="p">The point to transform.</param>
        /// <returns>A Point3D whose coefficients are the dot product of p and
        /// rows of m.</returns>
        /// <exception cref="System.ArgumentNullException">m or p are null.</exception>
        public static Point3D Transform(Matrix3D m, Point3D p)
        {
            return m * p;
        }

        /// <summary>
        /// Gets a coefficient in the matrix.
        /// </summary>
        /// <param name="row">The row of the coefficient to retrieve.</param>
        /// <param name="col">The column of the coefficient to retrieve.</param>
        /// <returns>The coefficient at the specified row and column.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">row or col are less than zero
        /// or greater than two.</exception>
        public Rational this[int row, int col]
        {
            get
            {
                if (0 > row || 2 < row)
                    throw new ArgumentOutOfRangeException("row");
                if (0 > col || 2 < col)
                    throw new ArgumentOutOfRangeException("col");

                return rows[row][col];
            }
        }

        /// <summary>
        /// Computes the inverse of this Matrix3D.
        /// </summary>
        /// <returns>The Matrix3D that, when multiplied by this Matrix3D, yields the
        /// Matrix3D.Identity.</returns>
        /// <exception cref="InvalidOperationException">This matrix is singular
        /// (i.e. its determinant is zero).</exception>
        public Matrix3D ComputeInverse()
        {
            return new Matrix3D(Matrix.Invert(3, rows));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Matrix.StringFormat(3, rows);
        }
    }
}