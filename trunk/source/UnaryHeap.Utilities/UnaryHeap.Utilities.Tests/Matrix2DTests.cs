using System;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class Matrix2DTests
    {
        [Fact]
        public void Identity()
        {
            var sut = Matrix2D.Identity;
            AssertMatrix(sut, 1, 0, 0, 1);

            for (int x = -5; x <= 5; x++)
                for (int y = -5; y <= 5; y++)
                    Assert.Equal(new Point2D(x, y), sut * new Point2D(x, y));
        }

        [Fact]
        public void ReflectionAlongXAxis()
        {
            var sut = Matrix2D.XReflection;
            AssertMatrix(sut, 1, 0, 0, -1);

            for (int x = -5; x <= 5; x++)
                for (int y = -5; y <= 5; y++)
                    Assert.Equal(new Point2D(x, -y), sut * new Point2D(x, y));
        }

        [Fact]
        public void ReflectionAlongYAxis()
        {
            var sut = Matrix2D.YReflection;
            AssertMatrix(sut, -1, 0, 0, 1);

            for (int x = -5; x <= 5; x++)
                for (int y = -5; y <= 5; y++)
                    Assert.Equal(new Point2D(-x, y), sut * new Point2D(x, y));
        }

        [Fact]
        public void HorizontalShear()
        {
            var sut = Matrix2D.HorizontalShear(2);
            AssertMatrix(sut, 1, 2, 0, 1);

            for (int y = -5; y <= 5; y++)
                Assert.Equal(new Point2D(2 * y, y), sut * new Point2D(0, y));
        }

        [Fact]
        public void VerticalShear()
        {
            var sut = Matrix2D.VerticalShear(3);
            AssertMatrix(sut, 1, 0, 3, 1);

            for (int x = -5; x <= 5; x++)
                Assert.Equal(new Point2D(x, 3 * x), sut * new Point2D(x, 0));
        }

        [Fact]
        public void Scale()
        {
            var sut1 = Matrix2D.Scale(5);
            AssertMatrix(sut1, 5, 0, 0, 5);
            var sut2 = 3 * Matrix2D.Identity;
            AssertMatrix(sut2, 3, 0, 0, 3);

            for (int x = -5; x <= 5; x++)
                for (int y = -5; y <= 5; y++)
                {
                    Assert.Equal(new Point2D(5 * x, 5 * y), sut1 * new Point2D(x, y));
                    Assert.Equal(new Point2D(3 * x, 3 * y), sut2 * new Point2D(x, y));
                }
        }

        [Fact]
        public void Inverse()
        {
            var m1 = new Matrix2D(1, 2, 3, 5);
            var m2 = new Matrix2D(-5, 2, 3, -1);

            AssertMatrix(m1.ComputeInverse(), -5, 2, 3, -1);
            AssertMatrix(m2.ComputeInverse(), 1, 2, 3, 5);
        }

        [Fact]
        public void MatrixMultiply()
        {
            var m1 = new Matrix2D(1, 2, 3, 5);
            var m2 = new Matrix2D(-5, 2, 3, -1);

            var sut1 = m1 * m2;
            var sut2 = m2 * m1;

            AssertMatrix(sut1, 1, 0, 0, 1);
            AssertMatrix(sut2, 1, 0, 0, 1);
        }

        void AssertMatrix(Matrix2D m,
            Rational c00, Rational c01,
            Rational c10, Rational c11)
        {
            Assert.Equal(c00, m[0, 0]);
            Assert.Equal(c01, m[0, 1]);
            Assert.Equal(c10, m[1, 0]);
            Assert.Equal(c11, m[1, 1]);
        }
    }

    public class LinearMappingTests
    {
        [Fact]
        public void TwoDimensions()
        {
            var src1 = new Point2D(1, 3);
            var src2 = new Point2D(2, -2);
            var dst1 = new Point2D(0, 3);
            var dst2 = new Point2D(1, 1);

            // The order that the points are specified should not affect the results
            var sut1 = LinearMapping.From(src1, src2).To(dst1, dst2);
            var sut2 = LinearMapping.From(src2, src1).To(dst2, dst1);

            Assert.Equal(dst1, sut1 * src1);
            Assert.Equal(dst2, sut1 * src2);
            Assert.Equal(dst1, sut2 * src1);
            Assert.Equal(dst2, sut2 * src2);
            Assert.Equal(Point2D.Origin, sut1 * Point2D.Origin);
            Assert.Equal(Point2D.Origin, sut2 * Point2D.Origin);
        }
    }

    public class AffineMappingTests
    {
        [Fact]
        public void OneDimension()
        {
            Rational src1 = 2;
            Rational src2 = 3;
            Rational dst1 = 6;
            Rational dst2 = 4;

            // The order that the points are specified should not affect the results
            var sut1 = AffineMapping.From(src1, src2).To(dst1, dst2);
            var sut2 = AffineMapping.From(src2, src1).To(dst2, dst1);

            Console.WriteLine(sut1);
            Console.WriteLine(sut2);

            Assert.Equal(dst1, (sut1 * src1.Homogenized()).Dehomogenized());
            Assert.Equal(dst2, (sut1 * src2.Homogenized()).Dehomogenized());
            Assert.Equal(dst1, (sut2 * src1.Homogenized()).Dehomogenized());
            Assert.Equal(dst2, (sut2 * src2.Homogenized()).Dehomogenized());
        }
    }

    static class HomogenityExtensions
    {
        public static Point2D Homogenized(this Rational value)
        {
            return new Point2D(value, 1);
        }

        public static Rational Dehomogenized(this Point2D value)
        {
            if (0 == value.Y)
                throw new InvalidOperationException(
                    "Point has zero homogeneous coefficient.");

            return value.X / value.Y;
        }
    }

    static class LinearMapping
    {
        public interface ILinearMapper2D
        {
            Matrix2D To(Point2D dst1, Point2D dst2);
        }

        class LinearMapper2D : ILinearMapper2D
        {
            Matrix2D sourceInverse;

            public LinearMapper2D(Point2D src1, Point2D src2)
            {
                try
                {
                    sourceInverse = new Matrix2D(src1.X, src2.X, src1.Y, src2.Y)
                        .ComputeInverse();
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException(
                        "Source points are linearly dependent; cannot invert.");
                }
            }

            public Matrix2D To(Point2D dst1, Point2D dst2)
            {
                var dest = new Matrix2D(dst1.X, dst2.X, dst1.Y, dst2.Y);
                return dest * sourceInverse;
            }
        }

        public static ILinearMapper2D From(Point2D src1, Point2D src2)
        {
            return new LinearMapper2D(src1, src2);
        }
    }

    static class AffineMapping
    {
        public interface IAffineMapper2D
        {
            Matrix2D To(Rational dst1, Rational dst2);
        }

        class AffineMapper2D : IAffineMapper2D
        {
            Matrix2D sourceInverse;

            public AffineMapper2D(Rational src1, Rational src2)
            {
                try
                {
                    sourceInverse = new Matrix2D(src1, src2, 1, 1)
                        .ComputeInverse();
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException(
                        "Source points are linearly dependent; cannot invert.");
                }
            }

            public Matrix2D To(Rational dst1, Rational dst2)
            {
                var dest = new Matrix2D(dst1, dst2, 1, 1);
                return dest * sourceInverse;
            }
        }

        public static IAffineMapper2D From(Rational src1, Rational src2)
        {
            return new AffineMapper2D(src1, src2);
        }
    }

    class Matrix2D
    {
        public static readonly Matrix2D Identity = new Matrix2D(1, 0, 0, 1);
        public static readonly Matrix2D XReflection = new Matrix2D(1, 0, 0, -1);
        public static readonly Matrix2D YReflection = new Matrix2D(-1, 0, 0, 1);

        public static Matrix2D HorizontalShear(Rational factor)
        {
            return new Matrix2D(1, factor, 0, 1);
        }

        public static Matrix2D VerticalShear(Rational factor)
        {
            return new Matrix2D(1, 0, factor, 1);
        }

        public static Matrix2D Scale(Rational factor)
        {
            return new Matrix2D(factor, 0, 0, factor);
        }

        Rational[][] rows;

        public Matrix2D(Rational r0c0, Rational r0c1, Rational r1c0, Rational r1c1)
        {
            rows = new[]
            {
                new[] { r0c0, r0c1 },
                new[] { r1c0, r1c1 }
            };
        }

        public static Matrix2D operator *(Matrix2D left, Matrix2D right)
        {
            return new Matrix2D(
                left.rows[0][0] * right.rows[0][0] + left.rows[0][1] * right.rows[1][0],
                left.rows[0][0] * right.rows[0][1] + left.rows[0][1] * right.rows[1][1],
                left.rows[1][0] * right.rows[0][0] + left.rows[1][1] * right.rows[1][0],
                left.rows[1][0] * right.rows[0][1] + left.rows[1][1] * right.rows[1][1]
            );
        }

        public static Matrix2D operator *(Rational c, Matrix2D m)
        {
            return new Matrix2D(
                c * m.rows[0][0], c * m.rows[0][1],
                c * m.rows[1][0], c * m.rows[1][1]
            );
        }

        public static Point2D operator *(Matrix2D m, Point2D p)
        {
            return new Point2D(RowMultiply(m.rows[0], p), RowMultiply(m.rows[1], p));
        }

        static Rational RowMultiply(Rational[] row, Point2D p)
        {
            return p.X * row[0] + p.Y * row[1];
        }

        public Rational this[int row, int col]
        {
            get { return rows[row][col]; }
        }

        public Matrix2D ComputeInverse()
        {
            var det = rows[0][0] * rows[1][1] - rows[0][1] * rows[1][0];

            if (0 == det)
                throw new InvalidOperationException("Matrix is singular.");

            return new Matrix2D(
                rows[1][1] / det,
                rows[0][1] / -det,
                rows[1][0] / -det,
                rows[0][0] / det
            );
        }

        public override string ToString()
        {
            return string.Format("[({0},{1}),({2},{3})]",
                rows[0][0], rows[0][1], rows[1][0], rows[1][1]);
        }
    }
}
