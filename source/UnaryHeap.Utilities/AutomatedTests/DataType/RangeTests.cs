using NUnit.Framework;
using System;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class RangeTests
    {
        [Test]
        public void Properties()
        {
            var sut = new DataType.Range(-3, 5);

            Assert.AreEqual((Rational)(-3), sut.Min);
            Assert.AreEqual((Rational)1, sut.Midpoint);
            Assert.AreEqual((Rational)5, sut.Max);
            Assert.AreEqual((Rational)8, sut.Size);
        }

        [Test]
        public void Padded()
        {
            var sut = new DataType.Range(-3, 5).GetPadded(2);

            Assert.AreEqual((Rational)(-5), sut.Min);
            Assert.AreEqual((Rational)1, sut.Midpoint);
            Assert.AreEqual((Rational)7, sut.Max);
            Assert.AreEqual((Rational)12, sut.Size);
        }

        [Test]
        public void Padded_ToZeroThickness()
        {
            var sut = new DataType.Range(-3, 5).GetPadded(-4);

            Assert.AreEqual((Rational)1, sut.Min);
            Assert.AreEqual((Rational)1, sut.Midpoint);
            Assert.AreEqual((Rational)1, sut.Max);
            Assert.AreEqual((Rational)0, sut.Size);
        }

        [Test]
        public void Padded_TooThin()
        {
            Assert.That(
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => { new DataType.Range(-3, 5).GetPadded(-5); })
                .Message.StartsWith(
                    "Specified thickness would result in a range with negative Size."));
        }

        [Test]
        public void Scaled()
        {
            var sut = new DataType.Range(-10, 8).GetScaled(10);

            Assert.AreEqual((Rational)(-91), sut.Min);
            Assert.AreEqual((Rational)(-1), sut.Midpoint);
            Assert.AreEqual((Rational)89, sut.Max);
            Assert.AreEqual((Rational)180, sut.Size);
        }

        [Test]
        public void Scaled_ToZeroThickness()
        {
            var sut = new DataType.Range(-10, 8).GetScaled(0);

            Assert.AreEqual((Rational)(-1), sut.Min);
            Assert.AreEqual((Rational)(-1), sut.Max);
            Assert.AreEqual((Rational)(-1), sut.Midpoint);
            Assert.AreEqual((Rational)0, sut.Size);
        }

        [Test]
        public void Scaled_NegativeFactor()
        {
            Assert.That(
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => { new DataType.Range(-3, 5).GetScaled(-1); })
                .Message.StartsWith(
                    "factor is negative."));
        }

        [Test]
        public void Contains()
        {
            var sut = new DataType.Range(-2, 4);

            Assert.False(sut.Contains(-3));
            Assert.True(sut.Contains(-2));
            Assert.True(sut.Contains(-1));

            Assert.True(sut.Contains(3));
            Assert.True(sut.Contains(4));
            Assert.False(sut.Contains(5));
        }

        [Test]
        public void CenteredAt()
        {
            var sut = new DataType.Range(-8, 2).CenteredAt(0);

            Assert.AreEqual((Rational)(-5), sut.Min);
            Assert.AreEqual((Rational)0, sut.Midpoint);
            Assert.AreEqual((Rational)5, sut.Max);
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { new DataType.Range(null, 1); });
            Assert.Throws<ArgumentNullException>(
                () => { new DataType.Range(1, null); });
            Assert.AreEqual("min is greater than max.", Assert.Throws<ArgumentException>(
                () => { new DataType.Range(1, -1); }).Message);

            Assert.Throws<ArgumentNullException>(
                () => { new DataType.Range(-1, 1).Contains(null); });
            Assert.Throws<ArgumentNullException>(
                () => { new DataType.Range(-1, 1).GetPadded(null); });
            Assert.Throws<ArgumentNullException>(
                () => { new DataType.Range(-1, 1).GetScaled(null); });
        }
    }
}
