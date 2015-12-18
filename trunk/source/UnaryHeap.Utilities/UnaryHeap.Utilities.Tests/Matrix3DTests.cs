using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D3;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class Matrix3DTests
    {
        [Fact]
        public void Identity()
        {
            var sut = Matrix3D.Identity;
            AssertMatrix(sut, 1, 0, 0, 0, 1, 0, 0, 0, 1);

            for (int x = -5; x <= 5; x++)
                for (int y = -5; y <= 5; y++)
                    for (int z = -5; z <= 5; z++)
                        Assert.Equal(new Point3D(x, y, z), sut * new Point3D(x, y, z));
        }

        [Fact]
        public void Inverse()
        {
            var m1 = new Matrix3D(3, 2, 3, 4, 5, 6, 7, 8, 10);
            var m2 = new Matrix3D(2, 4, -3, 2, 9, -6, -3, -10, 7);

            AssertMatrix(m1.ComputeInverse(), 2, 4, -3, 2, 9, -6, -3, -10, 7);
            AssertMatrix(m2.ComputeInverse(), 3, 2, 3, 4, 5, 6, 7, 8, 10);
        }

        [Fact]
        public void MatrixMultiply()
        {
            var m1 = new Matrix3D(3, 2, 3, 4, 5, 6, 7, 8, 10);
            var m2 = new Matrix3D(2, 4, -3, 2, 9, -6, -3, -10, 7);

            AssertMatrix(m1 * m2, 1, 0, 0, 0, 1, 0, 0, 0, 1);
            AssertMatrix(m2 * m1, 1, 0, 0, 0, 1, 0, 0, 0, 1);
        }

        [Fact]
        public void SingularMatrix()
        {
            var sut = new Matrix3D(1, 2, 3, 4, 5, 6, 7, 8, 9);

            Assert.Equal("Matrix is singular.",
                Assert.Throws<InvalidOperationException>(
                    () => { sut.ComputeInverse(); }).Message);
        }

        [Fact]
        public void StringRepresentation()
        {
            var sut = new Matrix3D(1, 2, 3, 4, 5, 6, 7, 8, 9);
            Assert.Equal("[[1,2,3];[4,5,6];[7,8,9]]", sut.ToString());
        }

        [Fact]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("elem00",
                () => { new Matrix3D(null, 0, 0, 0, 0, 0, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>("elem01",
                () => { new Matrix3D(0, null, 0, 0, 0, 0, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>("elem02",
                () => { new Matrix3D(0, 0, null, 0, 0, 0, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>("elem10",
                () => { new Matrix3D(0, 0, 0, null, 0, 0, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>("elem11",
                () => { new Matrix3D( 0, 0, 0, 0, null,0, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>("elem12",
                () => { new Matrix3D(0, 0, 0, 0, 0, null, 0, 0, 0); });
            Assert.Throws<ArgumentNullException>("elem20",
                () => { new Matrix3D(0, 0, 0, 0, 0, 0, null, 0, 0); });
            Assert.Throws<ArgumentNullException>("elem21",
                () => { new Matrix3D(0, 0, 0, 0, 0, 0, 0, null, 0); });
            Assert.Throws<ArgumentNullException>("elem22",
                () => { new Matrix3D(0, 0, 0, 0, 0, 0, 0, 0, null); });

            Assert.Throws<ArgumentNullException>("left",
                () => { var sut = ((Matrix3D)null) * Matrix3D.Identity; });
            Assert.Throws<ArgumentNullException>("right",
                () => { var sut = Matrix3D.Identity * ((Matrix3D)null); });

            Assert.Throws<ArgumentNullException>("m",
                () => { var sut = ((Matrix3D)null) * Point3D.Origin; });
            Assert.Throws<ArgumentNullException>("p",
                () => { var sut = Matrix3D.Identity * ((Point3D)null); });

            Assert.Throws<ArgumentOutOfRangeException>("row",
                () => { var sut = Matrix3D.Identity[-1, 0]; });
            Assert.Throws<ArgumentOutOfRangeException>("row",
                () => { var sut = Matrix3D.Identity[3, 0]; });
            Assert.Throws<ArgumentOutOfRangeException>("col",
                () => { var sut = Matrix3D.Identity[0, -1]; });
            Assert.Throws<ArgumentOutOfRangeException>("col",
                () => { var sut = Matrix3D.Identity[0, 3]; });
        }

        private void AssertMatrix(Matrix3D m,
            int elem00, int elem01, int elem02,
            int elem10, int elem11, int elem12,
            int elem20, int elem21, int elem22)
        {
            Assert.Equal(elem00, m[0,0]);
            Assert.Equal(elem01, m[0,1]);
            Assert.Equal(elem02, m[0,2]);
            Assert.Equal(elem10, m[1,0]);
            Assert.Equal(elem11, m[1,1]);
            Assert.Equal(elem12, m[1,2]);
            Assert.Equal(elem20, m[2,0]);
            Assert.Equal(elem21, m[2,1]);
            Assert.Equal(elem22, m[2,2]);
        }
    }
}
