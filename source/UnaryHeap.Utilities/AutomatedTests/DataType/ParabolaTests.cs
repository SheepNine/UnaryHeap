using NUnit.Framework;
using System;
using System.Linq;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class ParabolaTests
    {
        [Test]
        public void CoefficientConstructor()
        {
            foreach (var a0 in Enumerable.Range(-5, 11))
            {
                var sut = new Parabola(1, 0, a0);
                Assert.AreEqual((Rational)1, sut.A);
                Assert.AreEqual((Rational)0, sut.B);
                Assert.AreEqual((Rational)a0, sut.C);

                Assert.AreEqual((Rational)(9 + a0), sut.Evaulate(-3));
                Assert.AreEqual((Rational)(4 + a0), sut.Evaulate(-2));
                Assert.AreEqual((Rational)(1 + a0), sut.Evaulate(-1));
                Assert.AreEqual((Rational)(0 + a0), sut.Evaulate(00));
                Assert.AreEqual((Rational)(1 + a0), sut.Evaulate(01));
                Assert.AreEqual((Rational)(4 + a0), sut.Evaulate(02));
                Assert.AreEqual((Rational)(9 + a0), sut.Evaulate(03));

                Assert.AreEqual((Rational)(-6), sut.EvaluateDerivative(-3));
                Assert.AreEqual((Rational)(-4), sut.EvaluateDerivative(-2));
                Assert.AreEqual((Rational)(-2), sut.EvaluateDerivative(-1));
                Assert.AreEqual((Rational)00, sut.EvaluateDerivative(00));
                Assert.AreEqual((Rational)02, sut.EvaluateDerivative(01));
                Assert.AreEqual((Rational)04, sut.EvaluateDerivative(02));
                Assert.AreEqual((Rational)06, sut.EvaluateDerivative(03));
            }
        }

        [Test]
        public void FocusDirectrixConstructor()
        {
            foreach (Rational focusX in Enumerable.Range(0, 10))
                foreach (Rational focusY in Enumerable.Range(0, 10))
                    foreach (Rational directrixY in Enumerable.Range(0, 10))
                    {
                        if (focusY == directrixY)
                            continue;

                        var focus = new Point2D(focusX, focusY);

                        var sut = Parabola.FromFocusDirectrix(focus, directrixY);

                        Assert.AreEqual(focus, sut.Focus);
                        Assert.AreEqual(directrixY, sut.DirectrixY);

                        Assert.AreEqual(focusY, sut.Evaulate(focusX + focusY - directrixY));
                        Assert.AreEqual(focusY, sut.Evaulate(focusX - focusY + directrixY));
                        Assert.AreEqual((focusY + directrixY) / 2,
                            sut.Evaulate(focusX));
                    }
        }

        [Test]
        public void Difference()
        {
            var a = new Parabola(1, 2, 3);
            var b = new Parabola(4, 5, 6);
            var sut = Parabola.Difference(b, a);

            foreach (var i in Enumerable.Range(0, 100))
                Assert.AreEqual(b.Evaulate(i) - a.Evaulate(i), sut.Evaulate(i));
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { new Parabola(null, 1, 1); });
            Assert.Throws<ArgumentNullException>(
                () => { new Parabola(1, null, 1); });
            Assert.Throws<ArgumentNullException>(
                () => { new Parabola(1, 1, null); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { new Parabola(Rational.Zero, 1, 1); });

            Assert.Throws<ArgumentNullException>(
                () => { Parabola.FromFocusDirectrix(null, Rational.Zero); });
            Assert.Throws<ArgumentNullException>(
                () => { Parabola.FromFocusDirectrix(Point2D.Origin, null); });
            Assert.Throws<ArgumentException>(
                () => { Parabola.FromFocusDirectrix(Point2D.Origin, Rational.Zero); });

            var sut = new Parabola(1, 0, 0);

            Assert.Throws<ArgumentNullException>(
                () => { sut.Evaulate(null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.EvaluateDerivative(null); });
        }
    }
}
