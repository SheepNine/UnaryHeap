using System;
using System.Linq;
using UnaryHeap.Utilities.Core;

namespace UnaryHeap.Utilities
{
    internal static class Matrix
    {
        public static Rational[][] Multiply(
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

        public static Rational[][] Invert(int rank, Rational[][] elements)
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
    }
}
