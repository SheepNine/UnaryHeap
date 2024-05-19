using NUnit.Framework;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Orthotope3DTests
    {
        [Test]
        public void Constructor_Values()
        {
            var sut = new Orthotope3D(1, 2, 3, 4, 5, 6);
            Assert.AreEqual((Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)2, sut.Y.Min);
            Assert.AreEqual((Rational)3, sut.Z.Min);
            Assert.AreEqual((Rational)4, sut.X.Max);
            Assert.AreEqual((Rational)5, sut.Y.Max);
            Assert.AreEqual((Rational)6, sut.Z.Max);
        }

        [Test]
        public void Constructor_Ranges()
        {
            var sut = new Orthotope3D(new Range(1, 2), new Range(3, 4), new Range(5, 6));
            Assert.AreEqual((Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)2, sut.X.Max);
            Assert.AreEqual((Rational)3, sut.Y.Min);
            Assert.AreEqual((Rational)4, sut.Y.Max);
            Assert.AreEqual((Rational)5, sut.Z.Min);
            Assert.AreEqual((Rational)6, sut.Z.Max);
        }

        [Test]
        public void FromPoints()
        {
            var sut = Orthotope3D.FromPoints(new[] {
                new Point3D(1, 5, 3), new Point3D(4, 2, 6) });
            Assert.AreEqual((Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)2, sut.Y.Min);
            Assert.AreEqual((Rational)3, sut.Z.Min);
            Assert.AreEqual((Rational)4, sut.X.Max);
            Assert.AreEqual((Rational)5, sut.Y.Max);
            Assert.AreEqual((Rational)6, sut.Z.Max);
        }

        [Test]
        public void FromOnePoint()
        {
            var sut = Orthotope3D.FromPoints(new[] { new Point3D(1, 2, 3) });
            Assert.AreEqual((Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)2, sut.Y.Min);
            Assert.AreEqual((Rational)3, sut.Z.Min);
            Assert.AreEqual((Rational)1, sut.X.Max);
            Assert.AreEqual((Rational)2, sut.Y.Max);
            Assert.AreEqual((Rational)3, sut.Z.Max);
        }

        [Test]
        public void Contains()
        {
            var sut = new Orthotope3D(1, 2, 3, 4, 5, 6);
            Assert.IsTrue(sut.Contains(new Point3D(2, 4, 5)));
            Assert.IsFalse(sut.Contains(new Point3D(0, 4, 5)));
            Assert.IsFalse(sut.Contains(new Point3D(5, 4, 5)));
            Assert.IsFalse(sut.Contains(new Point3D(2, 1, 5)));
            Assert.IsFalse(sut.Contains(new Point3D(2, 6, 5)));
            Assert.IsFalse(sut.Contains(new Point3D(2, 4, 2)));
            Assert.IsFalse(sut.Contains(new Point3D(2, 4, 7)));
        }

        [Test]
        public void GetPadded()
        {
            var sut = new Orthotope3D(1, 2, 3, 4, 5, 6).GetPadded(4);
            Assert.AreEqual(-(Rational)3, sut.X.Min);
            Assert.AreEqual(-(Rational)2, sut.Y.Min);
            Assert.AreEqual(-(Rational)1, sut.Z.Min);
            Assert.AreEqual((Rational)8, sut.X.Max);
            Assert.AreEqual((Rational)9, sut.Y.Max);
            Assert.AreEqual((Rational)10, sut.Z.Max);
        }

        [Test]
        public void GetScaled()
        {
            var sut = new Orthotope3D(1, 2, 3, 5, 6, 7).GetScaled(2);
            Assert.AreEqual(-(Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)0, sut.Y.Min);
            Assert.AreEqual((Rational)1, sut.Z.Min);
            Assert.AreEqual((Rational)7, sut.X.Max);
            Assert.AreEqual((Rational)8, sut.Y.Max);
            Assert.AreEqual((Rational)9, sut.Z.Max);
        }

        [Test]
        public void CenteredAt()
        {
            var sut = new Orthotope3D(1, 2, 3, 5, 6, 7).CenteredAt(new Point3D(7, 8, 9));
            Assert.AreEqual((Rational)5, sut.X.Min);
            Assert.AreEqual((Rational)6, sut.Y.Min);
            Assert.AreEqual((Rational)7, sut.Z.Min);
            Assert.AreEqual((Rational)9, sut.X.Max);
            Assert.AreEqual((Rational)10, sut.Y.Max);
            Assert.AreEqual((Rational)11, sut.Z.Max);
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
        }
    }
}
