using NUnit.Framework;
using System;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Matrix3DTests
    {
        [Test]
        public void Identity()
        {
            var sut = Matrix3D.Identity;
            AssertMatrix(sut, 1, 0, 0, 0, 1, 0, 0, 0, 1);

            for (int x = -5; x <= 5; x++)
                for (int y = -5; y <= 5; y++)
                    for (int z = -5; z <= 5; z++)
                        Assert.AreEqual(new Point3D(x, y, z), sut * new Point3D(x, y, z));
        }

        [Test]
        public void Inverse()
        {
            var m1 = new Matrix3D(3, 2, 3, 4, 5, 6, 7, 8, 10);
            var m2 = new Matrix3D(2, 4, -3, 2, 9, -6, -3, -10, 7);

            AssertMatrix(m1.ComputeInverse(), 2, 4, -3, 2, 9, -6, -3, -10, 7);
            AssertMatrix(m2.ComputeInverse(), 3, 2, 3, 4, 5, 6, 7, 8, 10);
        }

        [Test]
        public void MatrixMultiply()
        {
            var m1 = new Matrix3D(3, 2, 3, 4, 5, 6, 7, 8, 10);
            var m2 = new Matrix3D(2, 4, -3, 2, 9, -6, -3, -10, 7);

            AssertMatrix(m1 * m2, 1, 0, 0, 0, 1, 0, 0, 0, 1);
            AssertMatrix(m2 * m1, 1, 0, 0, 0, 1, 0, 0, 0, 1);
        }

        [Test]
        public void SingularMatrix()
        {
            var sut = new Matrix3D(1, 2, 3, 4, 5, 6, 7, 8, 9);

            Assert.AreEqual("Matrix is singular.",
                Assert.Throws<InvalidOperationException>(
                    () => { sut.ComputeInverse(); }).Message);
        }

        [Test]
        public void StringRepresentation()
        {
            var sut = new Matrix3D(1, 2, 3, 4, 5, 6, 7, 8, 9);
            Assert.AreEqual("[[1,2,3];[4,5,6];[7,8,9]]", sut.ToString());
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { new Matrix3D(null, 0, 0, 0, 0, 0, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Matrix3D(0, null, 0, 0, 0, 0, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Matrix3D(0, 0, null, 0, 0, 0, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Matrix3D(0, 0, 0, null, 0, 0, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Matrix3D(0, 0, 0, 0, null, 0, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Matrix3D(0, 0, 0, 0, 0, null, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Matrix3D(0, 0, 0, 0, 0, 0, null, 0, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Matrix3D(0, 0, 0, 0, 0, 0, 0, null, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Matrix3D(0, 0, 0, 0, 0, 0, 0, 0, null); });

            Assert.Throws<ArgumentNullException>(
                () => { var sut = ((Matrix3D)null) * Matrix3D.Identity; });
            Assert.Throws<ArgumentNullException>(
                () => { var sut = Matrix3D.Identity * ((Matrix3D)null); });

            Assert.Throws<ArgumentNullException>(
                () => { var sut = ((Matrix3D)null) * Point3D.Origin; });
            Assert.Throws<ArgumentNullException>(
                () => { var sut = Matrix3D.Identity * ((Point3D)null); });

            Assert.Throws<ArgumentOutOfRangeException>(
                () => { var sut = Matrix3D.Identity[-1, 0]; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { var sut = Matrix3D.Identity[3, 0]; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { var sut = Matrix3D.Identity[0, -1]; });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { var sut = Matrix3D.Identity[0, 3]; });
        }

        private void AssertMatrix(Matrix3D m,
            int elem00, int elem01, int elem02,
            int elem10, int elem11, int elem12,
            int elem20, int elem21, int elem22)
        {
            Assert.AreEqual((Rational)elem00, m[0, 0]);
            Assert.AreEqual((Rational)elem01, m[0, 1]);
            Assert.AreEqual((Rational)elem02, m[0, 2]);
            Assert.AreEqual((Rational)elem10, m[1, 0]);
            Assert.AreEqual((Rational)elem11, m[1, 1]);
            Assert.AreEqual((Rational)elem12, m[1, 2]);
            Assert.AreEqual((Rational)elem20, m[2, 0]);
            Assert.AreEqual((Rational)elem21, m[2, 1]);
            Assert.AreEqual((Rational)elem22, m[2, 2]);
        }
    }
}
