using System;
using UnaryHeap.DataType;
using NUnit.Framework;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class Orthotope2DTests
    {
        [Test]
        public void Constructor_Values()
        {
            var sut = new Orthotope2D(1, 2, 3, 4);

            Assert.AreEqual((Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)2, sut.Y.Min);
            Assert.AreEqual((Rational)3, sut.X.Max);
            Assert.AreEqual((Rational)4, sut.Y.Max);
        }

        [Test]
        public void Constructor_Ranges()
        {
            var sut = new Orthotope2D(new DataType.Range(1, 2), new DataType.Range(3, 4));

            Assert.AreEqual((Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)2, sut.X.Max);
            Assert.AreEqual((Rational)3, sut.Y.Min);
            Assert.AreEqual((Rational)4, sut.Y.Max);
        }

        [Test]
        public void FromPoints()
        {
            var sut = Orthotope2D.FromPoints(new[] {
                new Point2D(-7, 2),
                new Point2D(-4, 3),
                new Point2D(-1, 4),
                new Point2D(02, 5),
                new Point2D(05, 6),
            });

            Assert.AreEqual((Rational)(-7), sut.X.Min);
            Assert.AreEqual((Rational)5, sut.X.Max);
            Assert.AreEqual((Rational)2, sut.Y.Min);
            Assert.AreEqual((Rational)6, sut.Y.Max);

            Assert.AreEqual(new Point2D(-1, 4), sut.Center);
        }

        [Test]
        public void FromOnePoint()
        {
            var sut = Orthotope2D.FromPoints(new[] {
                new Point2D(-7, 2),
            });

            Assert.AreEqual((Rational)(-7), sut.X.Min);
            Assert.AreEqual((Rational)(-7), sut.X.Max);
            Assert.AreEqual((Rational)2, sut.Y.Min);
            Assert.AreEqual((Rational)2, sut.Y.Max);
        }

        [Test]
        public void Contains()
        {
            var sut = new Orthotope2D(0, 10, 10, 20);

            Assert.True(sut.Contains(new Point2D(5, 15)));
            Assert.False(sut.Contains(new Point2D(15, 15)));
            Assert.False(sut.Contains(new Point2D(-5, 15)));
            Assert.False(sut.Contains(new Point2D(5, 25)));
            Assert.False(sut.Contains(new Point2D(5, 5)));
        }

        [Test]
        public void GetPadded()
        {
            var sut = new Orthotope2D(0, 10, 20, 30).GetPadded(3);

            Assert.AreEqual((Rational)(-3), sut.X.Min);
            Assert.AreEqual((Rational)7, sut.Y.Min);
            Assert.AreEqual((Rational)23, sut.X.Max);
            Assert.AreEqual((Rational)33, sut.Y.Max);
        }

        [Test]
        public void GetScaled()
        {
            var sut = new Orthotope2D(-1, -3, 1, 3).GetScaled(2);

            Assert.AreEqual((Rational)(-2), sut.X.Min);
            Assert.AreEqual((Rational)(-6), sut.Y.Min);
            Assert.AreEqual((Rational)2, sut.X.Max);
            Assert.AreEqual((Rational)6, sut.Y.Max);
        }

        [Test]
        public void CenteredAt()
        {
            var sut = new Orthotope2D(-1, -3, 1, 3)
                .CenteredAt(new Point2D(1, 2));

            Assert.AreEqual((Rational)0, sut.X.Min);
            Assert.AreEqual((Rational)(-1), sut.Y.Min);
            Assert.AreEqual((Rational)2, sut.X.Max);
            Assert.AreEqual((Rational)5, sut.Y.Max);
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { new Orthotope2D(null, new DataType.Range(-1, 1)); });
            Assert.Throws<ArgumentNullException>(
                () => { new Orthotope2D(new DataType.Range(-1, 1), null); });

            Assert.Throws<ArgumentNullException>(
                () => { new Orthotope2D(0, 0, 0, 0).Contains(null); });
            Assert.Throws<ArgumentNullException>(
                () => { new Orthotope2D(0, 0, 0, 0).GetPadded(null); });
            Assert.Throws<ArgumentNullException>(
                () => { new Orthotope2D(0, 0, 0, 0).GetScaled(null); });
            Assert.Throws<ArgumentNullException>(
                () => { new Orthotope2D(0, 0, 0, 0).CenteredAt(null); });

            Assert.Throws<ArgumentNullException>(
                () => { Orthotope2D.FromPoints(null); });
            Assert.Throws<ArgumentNullException>(
                () => { Orthotope2D.FromPoints(new Point2D[] { null }); });
            Assert.Throws<ArgumentException>(
                () => { Orthotope2D.FromPoints(new Point2D[] { }); });
        }
    }
}
