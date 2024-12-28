using NUnit.Framework;
using System;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Matrix4DTests
    {
        [Test]
        public void Identity()
        {
            var sut = Matrix4D.Identity;
            AssertMatrix(sut, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            for (int x = -5; x <= 5; x++)
                for (int y = -5; y <= 5; y++)
                    for (int z = -5; z <= 5; z++)
                        for (int w = -5; w <= 5; w++)
                        {
                            Assert.AreEqual(new Point4D(x, y, z, w),
                                sut * new Point4D(x, y, z, w));

                            Assert.AreEqual(new Point4D(x, y, z, w),
                                Matrix4D.Transform(sut, new Point4D(x, y, z, w)));
                        }
        }

        [Test]
        public void Inverse()
        {
            AssertMatrix(Matrix4D.Identity.ComputeInverse(),
                1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            var m1 = new Matrix4D(1, 2, 3, 4, 0, 1, 7, 8, 0, 0, 1, 12, 0, 0, 0, 1);
            var m2 = new Matrix4D(1, -2, 11, -120, 0, 1, -7, 76, 0, 0, 1, -12, 0, 0, 0, 1);

            AssertMatrix(m1.ComputeInverse(),
                1, -2, 11, -120, 0, 1, -7, 76, 0, 0, 1, -12, 0, 0, 0, 1);
            AssertMatrix(m2.ComputeInverse(),
                1, 2, 3, 4, 0, 1, 7, 8, 0, 0, 1, 12, 0, 0, 0, 1);
        }

        [Test]
        public void MatrixMultiply()
        {
            var m1 = new Matrix4D(1, 2, 3, 4, 0, 1, 7, 8, 0, 0, 1, 12, 0, 0, 0, 1);
            var m2 = new Matrix4D(1, -2, 11, -120, 0, 1, -7, 76, 0, 0, 1, -12, 0, 0, 0, 1);

            AssertMatrix(m1 * m2,
                1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            AssertMatrix(Matrix4D.Multiply(m2, m1),
                1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        }

        [Test]
        public void SingularMatrix()
        {
            var sut = new Matrix4D(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);

            Assert.AreEqual("Matrix is singular.",
                Assert.Throws<InvalidOperationException>(
                    () => { sut.ComputeInverse(); }).Message);
        }

        [Test]
        public void StringRepresentation()
        {
            var sut = new Matrix4D(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
            Assert.AreEqual("[[1,2,3,4];[5,6,7,8];[9,10,11,12];[13,14,15,16]]", sut.ToString());
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            var m = Matrix4D.Identity;
            var p = Point4D.Origin;

            TestUtils.NullChecks(new()
            {
                { typeof(ArgumentNullException), new TestDelegate[] {
                    () => { new Matrix4D(null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0); },
                    () => { new Matrix4D(0, null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0); },
                    () => { new Matrix4D(0, 0, null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0); },
                    () => { new Matrix4D(0, 0, 0, null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0); },
                    () => { new Matrix4D(0, 0, 0, 0, null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0); },
                    () => { new Matrix4D(0, 0, 0, 0, 0, null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0); },
                    () => { new Matrix4D(0, 0, 0, 0, 0, 0, null, 0, 0, 0, 0, 0, 0, 0, 0, 0); },
                    () => { new Matrix4D(0, 0, 0, 0, 0, 0, 0, null, 0, 0, 0, 0, 0, 0, 0, 0); },
                    () => { new Matrix4D(0, 0, 0, 0, 0, 0, 0, 0, null, 0, 0, 0, 0, 0, 0, 0); },
                    () => { new Matrix4D(0, 0, 0, 0, 0, 0, 0, 0, 0, null, 0, 0, 0, 0, 0, 0); },
                    () => { new Matrix4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null, 0, 0, 0, 0, 0); },
                    () => { new Matrix4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null, 0, 0, 0, 0); },
                    () => { new Matrix4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null, 0, 0, 0); },
                    () => { new Matrix4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null, 0, 0); },
                    () => { new Matrix4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null, 0); },
                    () => { new Matrix4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null); },
                    () => { _ = (Matrix4D)null * m; },
                    () => { _ = m * (Matrix4D)null; },
                    () => { _ = (Matrix4D)null * p; },
                    () => { _ = m * (Point4D)null; },
                }},
                { typeof(ArgumentOutOfRangeException), new TestDelegate[]
                {
                    () => { _ = m[-1, 0]; },
                    () => { _ = m[4, 0]; },
                    () => { _ = m[0, -1]; },
                    () => { _ = m[0, 4]; }
                }}
            });
        }

        static void AssertMatrix(Matrix4D m,
            int elem00, int elem01, int elem02, int elem03,
            int elem10, int elem11, int elem12, int elem13,
            int elem20, int elem21, int elem22, int elem23,
            int elem30, int elem31, int elem32, int elem33)
        {
            Assert.AreEqual((Rational)elem00, m[0, 0]);
            Assert.AreEqual((Rational)elem01, m[0, 1]);
            Assert.AreEqual((Rational)elem02, m[0, 2]);
            Assert.AreEqual((Rational)elem03, m[0, 3]);
            Assert.AreEqual((Rational)elem10, m[1, 0]);
            Assert.AreEqual((Rational)elem11, m[1, 1]);
            Assert.AreEqual((Rational)elem12, m[1, 2]);
            Assert.AreEqual((Rational)elem13, m[1, 3]);
            Assert.AreEqual((Rational)elem20, m[2, 0]);
            Assert.AreEqual((Rational)elem21, m[2, 1]);
            Assert.AreEqual((Rational)elem22, m[2, 2]);
            Assert.AreEqual((Rational)elem23, m[2, 3]);
            Assert.AreEqual((Rational)elem30, m[3, 0]);
            Assert.AreEqual((Rational)elem31, m[3, 1]);
            Assert.AreEqual((Rational)elem32, m[3, 2]);
            Assert.AreEqual((Rational)elem33, m[3, 3]);
        }
    }
}
