﻿using System;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.D3;
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
            var sut = Matrix2D.XShear(2);
            AssertMatrix(sut, 1, 2, 0, 1);

            for (int y = -5; y <= 5; y++)
                Assert.Equal(new Point2D(2 * y, y), sut * new Point2D(0, y));
        }

        [Fact]
        public void VerticalShear()
        {
            var sut = Matrix2D.YShear(3);
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

        [Fact]
        public void SingularMatrix()
        {
            var sut = new Matrix2D(1, 2, 2, 4);

            Assert.Equal("Matrix is singular.",
                Assert.Throws<InvalidOperationException>(
                () => { sut.ComputeInverse(); }).Message);
        }

        [Fact]
        public void StringRepresentation()
        {
            var sut = new Matrix2D(1, 2, 3, 4);
            Assert.Equal("[[1,2];[3,4]]", sut.ToString());
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

        [Fact]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("factor",
                () => { Matrix2D.XShear(null); });
            Assert.Throws<ArgumentNullException>("factor",
                () => { Matrix2D.YShear(null); });
            Assert.Throws<ArgumentNullException>("factor",
                () => { Matrix2D.Scale(null); });

            Assert.Throws<ArgumentNullException>("elem00",
                () => { new Matrix2D(null, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>("elem01",
                () => { new Matrix2D(0, null, 0, 0); });
            Assert.Throws<ArgumentNullException>("elem10",
                () => { new Matrix2D(0, 0, null, 0); });
            Assert.Throws<ArgumentNullException>("elem11",
                () => { new Matrix2D(0, 0, 0, null); });

            Assert.Throws<ArgumentNullException>("left",
                () => { var sut = ((Matrix2D)null) * Matrix2D.Identity; });
            Assert.Throws<ArgumentNullException>("right",
                () => { var sut = Matrix2D.Identity * ((Matrix2D)null); });

            Assert.Throws<ArgumentNullException>("m",
                () => { var sut = ((Matrix2D)null) * Point2D.Origin; });
            Assert.Throws<ArgumentNullException>("p",
                () => { var sut = Matrix2D.Identity * ((Point2D)null); });

            Assert.Throws<ArgumentNullException>("c",
                () => { var sut = ((Rational)null) * Matrix2D.Identity; });
            Assert.Throws<ArgumentNullException>("m",
                () => { var sut = Rational.Zero * ((Matrix2D)null); });

            Assert.Throws<ArgumentOutOfRangeException>("row",
                () => { var sut = Matrix2D.Identity[-1, 0]; });
            Assert.Throws<ArgumentOutOfRangeException>("row",
                () => { var sut = Matrix2D.Identity[2, 0]; });
            Assert.Throws<ArgumentOutOfRangeException>("col",
                () => { var sut = Matrix2D.Identity[0, -1]; });
            Assert.Throws<ArgumentOutOfRangeException>("col",
                () => { var sut = Matrix2D.Identity[0, 2]; });
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
        #region 2D Mapping

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

        #endregion


        #region 3D Mapping

        public interface ILinearMapper3D
        {
            Matrix3D To(Point3D dst1, Point3D dst2, Point3D dst3);
        }

        class LinearMapper3D : ILinearMapper3D
        {
            Matrix3D sourceInverse;

            public LinearMapper3D(Point3D src1, Point3D src2, Point3D src3)
            {
                try
                {
                    sourceInverse = new Matrix3D(
                        src1.X, src2.X, src3.X,
                        src1.Y, src2.Y, src3.Y,
                        src1.Z, src2.Z, src3.Z)
                        .ComputeInverse();
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException(
                        "Source points are linearly dependent; cannot invert.");
                }
            }

            public Matrix3D To(Point3D dst1, Point3D dst2, Point3D dst3)
            {
                var dest = new Matrix3D(
                    dst1.X, dst2.X, dst3.X,
                    dst1.Y, dst2.Y, dst3.Y,
                    dst1.Z, dst2.Z, dst3.Z);

                return dest * sourceInverse;
            }
        }

        public static ILinearMapper3D From(Point3D src1, Point3D src2, Point3D src3)
        {
            return new LinearMapper3D(src1, src2, src3);
        }

        #endregion
    }

    static class AffineMapping
    {
        #region 1D Mapping

        public interface IAffineMapper1D
        {
            Matrix2D To(Rational dst1, Rational dst2);
        }

        class AffineMapper1D : IAffineMapper1D
        {
            Matrix2D sourceInverse;

            public AffineMapper1D(Rational src1, Rational src2)
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

        public static IAffineMapper1D From(Rational src1, Rational src2)
        {
            return new AffineMapper1D(src1, src2);
        }


        #endregion


        #region 2D Mapping

        public interface IAffineMapper2D
        {
            Matrix3D To(Point2D dst1, Point2D dst2, Point2D dst3);
        }

        class AffineMapper2D : IAffineMapper2D
        {
            Matrix3D sourceInverse;

            public AffineMapper2D(Point2D src1, Point2D src2, Point2D src3)
            {
                try
                {
                    sourceInverse = new Matrix3D(
                        src1.X, src2.X, src3.X,
                        src1.Y, src2.Y, src3.Y,
                        1, 1, 1)
                        .ComputeInverse();
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException(
                        "Source points are linearly dependent; cannot invert.");
                }
            }

            public Matrix3D To(Point2D dst1, Point2D dst2, Point2D dst3)
            {
                var dest = new Matrix3D(
                    dst1.X, dst2.X, dst3.X,
                    dst1.Y, dst2.Y, dst3.Y,
                    1, 1, 1);

                return dest * sourceInverse;
            }
        }

        public static IAffineMapper2D From(Point2D src1, Point2D src2, Point2D src3)
        {
            return new AffineMapper2D(src1, src2, src3);
        }

        #endregion
    }

    class Matrix3D
    {
        public Matrix3D(
            Rational elem00, Rational elem01, Rational elem02,
            Rational elem10, Rational elem11, Rational elem12,
            Rational elem20, Rational elem21, Rational elem22)
        {
            throw new NotImplementedException();
        }

        public static Matrix3D operator * (Matrix3D left, Matrix3D right)
        {
            throw new NotImplementedException();
        }

        public Matrix3D ComputeInverse()
        {
            throw new NotImplementedException();
        }
    }
}
