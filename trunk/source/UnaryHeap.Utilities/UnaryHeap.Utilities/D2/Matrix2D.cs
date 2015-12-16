using System;
using System.Globalization;
using System.Linq;
using UnaryHeap.Utilities.Core;

namespace UnaryHeap.Utilities.D2
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "Matrix2D is immutable.")]
        public static readonly Matrix2D Identity = new Matrix2D(1, 0, 0, 1);

        /// <summary>
        /// Returns the two-dimensional matrix that inverts the Y-coordinate of input points.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "Matrix2D is immutable.")]
        public static readonly Matrix2D XReflection = new Matrix2D(1, 0, 0, -1);

        /// <summary>
        /// Returns the two-dimensional matrix that inverts the X-coordinate of input points.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "Matrix2D is immutable.")]
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
            if (null == factor)
                throw new ArgumentNullException("factor");

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

            if (null == factor)
                throw new ArgumentNullException("factor");

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
            if (null == factor)
                throw new ArgumentNullException("factor");

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
            if (null == elem00)
                throw new ArgumentNullException("elem00");
            if (null == elem01)
                throw new ArgumentNullException("elem01");
            if (null == elem10)
                throw new ArgumentNullException("elem10");
            if (null == elem11)
                throw new ArgumentNullException("elem11");

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
            if (null == left)
                throw new ArgumentNullException("left");
            if (null == right)
                throw new ArgumentNullException("right");

            return new Matrix2D(MatrixMultiply(2, left.rows, right.rows));
        }

        static Rational[][] MatrixMultiply(
            int rank, Rational[][] leftElements, Rational[][] rightElements)
        {
            var result = new Rational[rank][];

            for (int row = 0; row < rank; row++)
            {
                result[row] = new Rational[rank];

                for (int col = 0; col < rank; col++)
                {
                    var total = Rational.Zero;

                    for (int i = 0; i < rank; i++)
                        total += leftElements[row][i] * rightElements[i][col];

                    result[row][col] = total;
                }
            }

            return result;
        }

        static Rational[][] MatrixInvert(int rank, Rational[][] elements)
        {
            var input = CopyCoefficients(rank, elements);
            var output = IdentityMatrixCoefficients(rank);

            for (int col = 0; col < rank; col++)
            {
                for (int row = col; row < rank; row++)
                {
                    if (0 != input[row][col])
                    {
                        var factor = input[row][col].Inverse;
                        MultiplyRow(rank, input[row], factor);
                        MultiplyRow(rank, output[row], factor);
                    }
                }

                var pivot = col;

                while (0 == input[pivot][col])
                {
                    pivot++;

                    if (pivot == rank)
                        throw new InvalidOperationException("Matrix is singular.");
                }

                Swap(ref input[pivot], ref input[col]);
                Swap(ref output[pivot], ref output[col]);


                if (0 == input[col][col])
                    throw new NotImplementedException(
                        "Pivot rows to put a non-zero value at the anchor point.");

                for (int row = col + 1; row < rank; row++)
                {
                    if (0 != input[row][col])
                    {
                        SubtractRow(rank, input[row], input[col]);
                        SubtractRow(rank, output[row], output[col]);
                    }
                }
            }

            for (int col = rank - 1; col >= 0; col--)
            {
                for (int row = col - 1; row >= 0; row--)
                {
                    var coeff = input[row][col];
                    SubtractRow(rank, input[row], input[col], coeff);
                    SubtractRow(rank, output[row], output[col], coeff);
                }
            }

            return output;
        }

        static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// left -= right;
        /// </summary>
        static void SubtractRow(int rank, Rational[] left, Rational[] right)
        {
            foreach (var i in Enumerable.Range(0, rank))
                left[i] -= right[i];
        }
        /// <summary>
        /// left -= c * right;
        /// </summary>
        static void SubtractRow(int rank, Rational[] left, Rational[] right, Rational c)
        {
            foreach (var i in Enumerable.Range(0, rank))
                left[i] -= c * right[i];
        }

        static void MultiplyRow(int rank, Rational[] rational, Rational factor)
        {
            foreach (var i in Enumerable.Range(0, rank))
                rational[i] *= factor;
        }

        static Rational[][] CopyCoefficients(int rank, Rational[][] elements)
        {
            var result = new Rational[rank][];

            for (int row = 0; row < rank; row++)
            {
                result[row] = new Rational[rank];
                for (int col = 0; col < rank; col++)
                    result[row][col] = elements[row][col];
            }

            return result;
        }

        static Rational[][] IdentityMatrixCoefficients(int rank)
        {
            var result = new Rational[rank][];

            for (int row = 0; row < rank; row++)
            {
                result[row] = new Rational[rank];
                for (int col = 0; col < rank; col++)
                {
                    if (row == col)
                        result[row][col] = 1;
                    else
                        result[row][col] = 0;
                }
            }

            return result;
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
            if (null == c)
                throw new ArgumentNullException("c");
            if (null == m)
                throw new ArgumentNullException("m");

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
            if (null == m)
                throw new ArgumentNullException("m");
            if (null == p)
                throw new ArgumentNullException("p");

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
                    throw new ArgumentOutOfRangeException("row");
                if (0 > col || 1 < col)
                    throw new ArgumentOutOfRangeException("col");

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
            return new Matrix2D(MatrixInvert(2, rows));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[[{0},{1}];[{2},{3}]]",
                rows[0][0], rows[0][1], rows[1][0], rows[1][1]);
        }

        #endregion
    }
}
