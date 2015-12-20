using System;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class Orthotope2DTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Constructor_Values()
        {
            var sut = new Orthotope2D(1, 2, 3, 4);

            Assert.Equal(1, sut.X.Min);
            Assert.Equal(2, sut.Y.Min);
            Assert.Equal(3, sut.X.Max);
            Assert.Equal(4, sut.Y.Max);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Constructor_Ranges()
        {
            var sut = new Orthotope2D(new Range(1, 2), new Range(3, 4));

            Assert.Equal(1, sut.X.Min);
            Assert.Equal(2, sut.X.Max);
            Assert.Equal(3, sut.Y.Min);
            Assert.Equal(4, sut.Y.Max);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void FromPoints()
        {
            var sut = Orthotope2D.FromPoints(new[] {
                new Point2D(-7, 2),
                new Point2D(-4, 3),
                new Point2D(-1, 4),
                new Point2D(02, 5),
                new Point2D(05, 6),
            });

            Assert.Equal(-7, sut.X.Min);
            Assert.Equal(5, sut.X.Max);
            Assert.Equal(2, sut.Y.Min);
            Assert.Equal(6, sut.Y.Max);

            Assert.Equal(new Point2D(-1, 4), sut.Center);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void FromOnePoint()
        {
            var sut = Orthotope2D.FromPoints(new[] {
                new Point2D(-7, 2),
            });

            Assert.Equal(-7, sut.X.Min);
            Assert.Equal(-7, sut.X.Max);
            Assert.Equal(2, sut.Y.Min);
            Assert.Equal(2, sut.Y.Max);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Contains()
        {
            var sut = new Orthotope2D(0, 10, 10, 20);

            Assert.True(sut.Contains(new Point2D(5, 15)));
            Assert.False(sut.Contains(new Point2D(15, 15)));
            Assert.False(sut.Contains(new Point2D(-5, 15)));
            Assert.False(sut.Contains(new Point2D(5, 25)));
            Assert.False(sut.Contains(new Point2D(5, 5)));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void GetPadded()
        {
            var sut = new Orthotope2D(0, 10, 20, 30).GetPadded(3);

            Assert.Equal(-3, sut.X.Min);
            Assert.Equal(7, sut.Y.Min);
            Assert.Equal(23, sut.X.Max);
            Assert.Equal(33, sut.Y.Max);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void GetScaled()
        {
            var sut = new Orthotope2D(-1, -3, 1, 3).GetScaled(2);

            Assert.Equal(-2, sut.X.Min);
            Assert.Equal(-6, sut.Y.Min);
            Assert.Equal(2, sut.X.Max);
            Assert.Equal(6, sut.Y.Max);
        }


        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("x",
                () => { new Orthotope2D(null, new Range(-1, 1)); });
            Assert.Throws<ArgumentNullException>("y",
                () => { new Orthotope2D(new Range(-1, 1), null); });

            Assert.Throws<ArgumentNullException>("value",
                () => { new Orthotope2D(0, 0, 0, 0).Contains(null); });
            Assert.Throws<ArgumentNullException>("thickness",
                () => { new Orthotope2D(0, 0, 0, 0).GetPadded(null); });
            Assert.Throws<ArgumentNullException>("factor",
                () => { new Orthotope2D(0, 0, 0, 0).GetScaled(null); });

            Assert.Throws<ArgumentNullException>("points",
                () => { Orthotope2D.FromPoints(null); });
            Assert.Throws<ArgumentNullException>("points",
                () => { Orthotope2D.FromPoints(new Point2D[] { null }); });
            Assert.Throws<ArgumentException>("points",
                () => { Orthotope2D.FromPoints(new Point2D[] { }); });
        }
    }
}
