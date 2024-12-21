using System;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a four-dimensional matrix.
    /// </summary>
    public class Matrix4D
    {
        /// <summary>
        /// Returns the four-dimensional identity matrix.
        /// </summary>
        public static readonly Matrix4D Identity = new Matrix4D(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);

        readonly Rational[][] rows;

        /// <summary>
        /// Initializes a new instance of the Matrix4D class
        /// </summary>
        /// <param name="elem00">The coefficient in the first row and first column</param>
        /// <param name="elem01">The coefficient in the first row and second column</param>
        /// <param name="elem02">The coefficient in the first row and third column</param>
        /// <param name="elem03">The coefficient in the first row and fourth column</param>
        /// <param name="elem10">The coefficient in the second row and first column</param>
        /// <param name="elem11">The coefficient in the second row and second column</param>
        /// <param name="elem12">The coefficient in the second row and third column</param>
        /// <param name="elem13">The coefficient in the second row and fourth column</param>
        /// <param name="elem20">The coefficient in the third row and first column</param>
        /// <param name="elem21">The coefficient in the third row and second column</param>
        /// <param name="elem22">The coefficient in the third row and third column</param>
        /// <param name="elem23">The coefficient in the third row and fourth column</param>
        /// <param name="elem30">The coefficient in the fourth row and first column</param>
        /// <param name="elem31">The coefficient in the fourth row and second column</param>
        /// <param name="elem32">The coefficient in the fourth row and third column</param>
        /// <param name="elem33">The coefficient in the fourth row and fourth column</param>
        /// <exception cref="System.ArgumentNullException">
        /// Any of the parameters are null.</exception>
        public Matrix4D(
            Rational elem00, Rational elem01, Rational elem02, Rational elem03,
            Rational elem10, Rational elem11, Rational elem12, Rational elem13,
            Rational elem20, Rational elem21, Rational elem22, Rational elem23,
            Rational elem30, Rational elem31, Rational elem32, Rational elem33)
        {
            ArgumentNullException.ThrowIfNull(elem00);
            ArgumentNullException.ThrowIfNull(elem01);
            ArgumentNullException.ThrowIfNull(elem02);
            ArgumentNullException.ThrowIfNull(elem03);
            ArgumentNullException.ThrowIfNull(elem10);
            ArgumentNullException.ThrowIfNull(elem11);
            ArgumentNullException.ThrowIfNull(elem12);
            ArgumentNullException.ThrowIfNull(elem13);
            ArgumentNullException.ThrowIfNull(elem20);
            ArgumentNullException.ThrowIfNull(elem21);
            ArgumentNullException.ThrowIfNull(elem22);
            ArgumentNullException.ThrowIfNull(elem23);
            ArgumentNullException.ThrowIfNull(elem30);
            ArgumentNullException.ThrowIfNull(elem31);
            ArgumentNullException.ThrowIfNull(elem32);
            ArgumentNullException.ThrowIfNull(elem33);

            rows = new[]
            {
                new[] { elem00, elem01, elem02, elem03 },
                new[] { elem10, elem11, elem12, elem13 },
                new[] { elem20, elem21, elem22, elem23 },
                new[] { elem30, elem31, elem32, elem33 },
            };
        }

        Matrix4D(Rational[][] rows)
        {
            this.rows = rows;
        }

        /// <summary>
        /// Computes the product of two matrices.
        /// </summary>
        /// <param name="left">The 'left' matrix of the computation.</param>
        /// <param name="right">The 'right' matrix of the computation.</param>
        /// <returns>A Matrix4D whose coefficients are computed by taking the dot products
        /// of rows from left and columns from right.</returns>
        /// <exception cref="System.ArgumentNullException">left or right are null.</exception>
        public static Matrix4D operator *(Matrix4D left, Matrix4D right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            return new Matrix4D(Matrix.Multiply(4, left.rows, right.rows));
        }

        /// <summary>
        /// Computes the product of two matrices.
        /// </summary>
        /// <param name="left">The 'left' matrix of the computation.</param>
        /// <param name="right">The 'right' matrix of the computation.</param>
        /// <returns>A Matrix4D whose coefficients are computed by taking the dot products
        /// of rows from left and columns from right.</returns>
        /// <exception cref="System.ArgumentNullException">left or right are null.</exception>
        public static Matrix4D Multiply(Matrix4D left, Matrix4D right)
        {
            return left * right;
        }

        /// <summary>
        /// Computes the linear transformation of a point.
        /// </summary>
        /// <param name="m">The matrix corresponding to the transformation.</param>
        /// <param name="p">The point to transform.</param>
        /// <returns>A Point4D whose coefficients are the dot product of p and
        /// rows of m.</returns>
        /// <exception cref="System.ArgumentNullException">m or p are null.</exception>
        public static Point4D operator *(Matrix4D m, Point4D p)
        {
            ArgumentNullException.ThrowIfNull(m);
            ArgumentNullException.ThrowIfNull(p);

            return new Point4D(
                RowMultiply(m.rows[0], p),
                RowMultiply(m.rows[1], p),
                RowMultiply(m.rows[2], p),
                RowMultiply(m.rows[3], p)
            );
        }


        static Rational RowMultiply(Rational[] row, Point4D p)
        {
            return p.X * row[0] + p.Y * row[1] + p.Z * row[2] + p.W * row[3];
        }

        /// <summary>
        /// Computes the linear transformation of a point.
        /// </summary>
        /// <param name="m">The matrix corresponding to the transformation.</param>
        /// <param name="p">The point to transform.</param>
        /// <returns>A Point4D whose coefficients are the dot product of p and
        /// rows of m.</returns>
        /// <exception cref="System.ArgumentNullException">m or p are null.</exception>
        public static Point4D Transform(Matrix4D m, Point4D p)
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
                if (0 > row || 3 < row)
                    throw new ArgumentOutOfRangeException(nameof(row));
                if (0 > col || 3 < col)
                    throw new ArgumentOutOfRangeException(nameof(col));

                return rows[row][col];
            }
        }

        /// <summary>
        /// Computes the inverse of this Matrix4D.
        /// </summary>
        /// <returns>The Matrix4D that, when multiplied by this Matrix4D, yields the
        /// Matrix4D.Identity.</returns>
        /// <exception cref="InvalidOperationException">This matrix is singular
        /// (i.e. its determinant is zero).</exception>
        public Matrix4D ComputeInverse()
        {
            return new Matrix4D(Matrix.Invert(4, rows));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Matrix.StringFormat(4, rows);
        }
    }
}
