using NUnit.Framework;
using System;
using UnaryHeap.DataType;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class Matrix2DTests
    {
        [Test]
        public void Identity()
        {
            var sut = Matrix2D.Identity;
            AssertMatrix(sut, 1, 0, 0, 1);

            for (int x = -5; x <= 5; x++)
                for (int y = -5; y <= 5; y++)
                    Assert.AreEqual(new Point2D(x, y), sut * new Point2D(x, y));
        }

        [Test]
        public void ReflectionAlongXAxis()
        {
            var sut = Matrix2D.XReflection;
            AssertMatrix(sut, 1, 0, 0, -1);

            for (int x = -5; x <= 5; x++)
                for (int y = -5; y <= 5; y++)
                    Assert.AreEqual(new Point2D(x, -y), sut * new Point2D(x, y));
        }

        [Test]
        public void ReflectionAlongYAxis()
        {
            var sut = Matrix2D.YReflection;
            AssertMatrix(sut, -1, 0, 0, 1);

            for (int x = -5; x <= 5; x++)
                for (int y = -5; y <= 5; y++)
                    Assert.AreEqual(new Point2D(-x, y), sut * new Point2D(x, y));
        }

        [Test]
        public void HorizontalShear()
        {
            var sut = Matrix2D.XShear(2);
            AssertMatrix(sut, 1, 2, 0, 1);

            for (int y = -5; y <= 5; y++)
                Assert.AreEqual(new Point2D(2 * y, y), sut * new Point2D(0, y));
        }

        [Test]
        public void VerticalShear()
        {
            var sut = Matrix2D.YShear(3);
            AssertMatrix(sut, 1, 0, 3, 1);

            for (int x = -5; x <= 5; x++)
                Assert.AreEqual(new Point2D(x, 3 * x), sut * new Point2D(x, 0));
        }

        [Test]
        public void Scale()
        {
            var sut1 = Matrix2D.Scale(5);
            AssertMatrix(sut1, 5, 0, 0, 5);
            var sut2 = 3 * Matrix2D.Identity;
            AssertMatrix(sut2, 3, 0, 0, 3);

            for (int x = -5; x <= 5; x++)
                for (int y = -5; y <= 5; y++)
                {
                    Assert.AreEqual(new Point2D(5 * x, 5 * y), sut1 * new Point2D(x, y));
                    Assert.AreEqual(new Point2D(3 * x, 3 * y), sut2 * new Point2D(x, y));
                }
        }

        [Test]
        public void Inverse()
        {
            var m1 = new Matrix2D(1, 2, 3, 5);
            var m2 = new Matrix2D(-5, 2, 3, -1);

            AssertMatrix(m1.ComputeInverse(), -5, 2, 3, -1);
            AssertMatrix(m2.ComputeInverse(), 1, 2, 3, 5);
        }

        [Test]
        public void MoreInverses()
        {
            var sut = new Matrix2D(0, 1, 2, 3).ComputeInverse();
            AssertMatrix(sut, new Rational(-3, 2), new Rational(1, 2), 1, 0);
            AssertMatrix(sut * sut.ComputeInverse(), 1, 0, 0, 1);
        }

        [Test]
        public void MatrixMultiply()
        {
            var m1 = new Matrix2D(1, 2, 3, 5);
            var m2 = new Matrix2D(-5, 2, 3, -1);

            var sut1 = m1 * m2;
            var sut2 = m2 * m1;

            AssertMatrix(sut1, 1, 0, 0, 1);
            AssertMatrix(sut2, 1, 0, 0, 1);
        }

        [Test]
        public void SingularMatrix()
        {
            var sut = new Matrix2D(1, 2, 2, 4);

            Assert.AreEqual("Matrix is singular.",
                Assert.Throws<InvalidOperationException>(
                () => { sut.ComputeInverse(); }).Message);
        }

        [Test]
        public void StringRepresentation()
        {
            var sut = new Matrix2D(1, 2, 3, 4);
            Assert.AreEqual("[[1,2];[3,4]]", sut.ToString());
        }

        void AssertMatrix(Matrix2D m,
            Rational c00, Rational c01,
            Rational c10, Rational c11)
        {
            Assert.AreEqual(c00, m[0, 0]);
            Assert.AreEqual(c01, m[0, 1]);
            Assert.AreEqual(c10, m[1, 0]);
            Assert.AreEqual(c11, m[1, 1]);
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { Matrix2D.XShear(null); });
            Assert.Throws<ArgumentNullException>(
                () => { Matrix2D.YShear(null); });
            Assert.Throws<ArgumentNullException>(
                () => { Matrix2D.Scale(null); });

            Assert.Throws<ArgumentNullException>(
                () => { new Matrix2D(null, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Matrix2D(0, null, 0, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Matrix2D(0, 0, null, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Matrix2D(0, 0, 0, null); });

            Assert.Throws<ArgumentNullException>(
                () => { var sut = ((Matrix2D)null) * Matrix2D.Identity; });
            Assert.Throws<ArgumentNullException>(
                () => { var sut = Matrix2D.Identity * ((Matrix2D)null); });

            Assert.Throws<ArgumentNullException>(
                () => { var sut = ((Matrix2D)null) * Point2D.Origin; });
            Assert.Throws<ArgumentNullException>(
                () => { var sut = Matrix2D.Identity * ((Point2D)null); });

            Assert.Throws<ArgumentNullException>(
                () => { var sut = ((Rational)null) * Matrix2D.Identity; });
            Assert.Throws<ArgumentNullException>(
                () => { var sut = Rational.Zero * ((Matrix2D)null); });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { var sut = Matrix2D.Identity[-1, 0]; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { var sut = Matrix2D.Identity[2, 0]; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { var sut = Matrix2D.Identity[0, -1]; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { var sut = Matrix2D.Identity[0, 2]; });
        }
    }

    static class HomogenityExtensions
    {
        public static Point2D Homogenized(this Rational value)
        {
            return new Point2D(value, 1);
        }

        public static Point3D Homogenized(this Point2D value)
        {
            return new Point3D(value.X, value.Y, 1);
        }

        public static Rational Dehomogenized(this Point2D value)
        {
            if (0 == value.Y)
                throw new InvalidOperationException(
                    "Point has zero homogeneous coefficient.");

            return value.X / value.Y;
        }

        public static Point2D Dehomogenized(this Point3D value)
        {
            if (0 == value.Z)
                throw new InvalidOperationException(
                    "Point has zero homogeneous coefficient.");

            return new Point2D(value.X / value.Z, value.Y / value.Z);
        }
    }
}
