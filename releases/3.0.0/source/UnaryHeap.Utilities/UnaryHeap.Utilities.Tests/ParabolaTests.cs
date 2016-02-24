using System;
using System.Linq;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class ParabolaTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void CoefficientConstructor()
        {
            foreach (var a0 in Enumerable.Range(-5, 11))
            {
                var sut = new Parabola(1, 0, a0);
                Assert.Equal(1, sut.A);
                Assert.Equal(0, sut.B);
                Assert.Equal(a0, sut.C);

                Assert.Equal(9 + a0, sut.Evaulate(-3));
                Assert.Equal(4 + a0, sut.Evaulate(-2));
                Assert.Equal(1 + a0, sut.Evaulate(-1));
                Assert.Equal(0 + a0, sut.Evaulate(00));
                Assert.Equal(1 + a0, sut.Evaulate(01));
                Assert.Equal(4 + a0, sut.Evaulate(02));
                Assert.Equal(9 + a0, sut.Evaulate(03));

                Assert.Equal(-6, sut.EvaluateDerivative(-3));
                Assert.Equal(-4, sut.EvaluateDerivative(-2));
                Assert.Equal(-2, sut.EvaluateDerivative(-1));
                Assert.Equal(00, sut.EvaluateDerivative(00));
                Assert.Equal(02, sut.EvaluateDerivative(01));
                Assert.Equal(04, sut.EvaluateDerivative(02));
                Assert.Equal(06, sut.EvaluateDerivative(03));
            }
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void FocusDirectrixConstructor()
        {
            foreach (var focusX in Enumerable.Range(0, 10))
                foreach (var focusY in Enumerable.Range(0, 10))
                    foreach (var directrixY in Enumerable.Range(0, 10))
                    {
                        if (focusY == directrixY)
                            continue;

                        var focus = new Point2D(focusX, focusY);

                        var sut = Parabola.FromFocusDirectrix(focus, directrixY);

                        Assert.Equal(focus, sut.Focus);
                        Assert.Equal(directrixY, sut.DirectrixY);

                        Assert.Equal(focusY, sut.Evaulate(focusX + focusY - directrixY));
                        Assert.Equal(focusY, sut.Evaulate(focusX - focusY + directrixY));
                        Assert.Equal(new Rational(focusY + directrixY) / 2, sut.Evaulate(focusX));
                    }
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Difference()
        {
            var a = new Parabola(1, 2, 3);
            var b = new Parabola(4, 5, 6);
            var sut = Parabola.Difference(b, a);

            foreach (var i in Enumerable.Range(0, 100))
                Assert.Equal(b.Evaulate(i) - a.Evaulate(i), sut.Evaulate(i));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("a",
                () => { new Parabola(null, 1, 1); });
            Assert.Throws<ArgumentNullException>("b",
                () => { new Parabola(1, null, 1); });
            Assert.Throws<ArgumentNullException>("c",
                () => { new Parabola(1, 1, null); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { new Parabola(Rational.Zero, 1, 1); });

            Assert.Throws<ArgumentNullException>("focus",
                () => { Parabola.FromFocusDirectrix(null, Rational.Zero); });
            Assert.Throws<ArgumentNullException>("directrixY",
                () => { Parabola.FromFocusDirectrix(Point2D.Origin, null); });
            Assert.Throws<ArgumentException>(
                () => { Parabola.FromFocusDirectrix(Point2D.Origin, Rational.Zero); });

            var sut = new Parabola(1, 0, 0);

            Assert.Throws<ArgumentNullException>("x",
                () => { sut.Evaulate(null); });
            Assert.Throws<ArgumentNullException>("x",
                () => { sut.EvaluateDerivative(null); });
        }
    }
}
