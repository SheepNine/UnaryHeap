using System;
using UnaryHeap.Utilities.Core;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class RangeTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Properties()
        {
            var sut = new Range(-3, 5);

            Assert.Equal(-3, sut.Min);
            Assert.Equal(1, sut.Midpoint);
            Assert.Equal(5, sut.Max);
            Assert.Equal(8, sut.Size);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Padded()
        {
            var sut = new Range(-3, 5).GetPadded(2);

            Assert.Equal(-5, sut.Min);
            Assert.Equal(1, sut.Midpoint);
            Assert.Equal(7, sut.Max);
            Assert.Equal(12, sut.Size);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Padded_ToZeroThickness()
        {
            var sut = new Range(-3, 5).GetPadded(-4);

            Assert.Equal(1, sut.Min);
            Assert.Equal(1, sut.Midpoint);
            Assert.Equal(1, sut.Max);
            Assert.Equal(0, sut.Size);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Padded_TooThin()
        {
            Assert.StartsWith("Specified thickness would result in a range with negative Size.",
                Assert.Throws<ArgumentOutOfRangeException>("thickness", () => { new Range(-3, 5).GetPadded(-5); }).Message);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Scaled()
        {
            var sut = new Range(-10, 8).GetScaled(10);

            Assert.Equal(-91, sut.Min);
            Assert.Equal(-1, sut.Midpoint);
            Assert.Equal(89, sut.Max);
            Assert.Equal(180, sut.Size);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Scaled_ToZeroThickness()
        {
            var sut = new Range(-10, 8).GetScaled(0);

            Assert.Equal(-1, sut.Min);
            Assert.Equal(-1, sut.Max);
            Assert.Equal(-1, sut.Midpoint);
            Assert.Equal(0, sut.Size);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Scaled_NegativeFactor()
        {
            Assert.StartsWith("factor is negative.",
                Assert.Throws<ArgumentOutOfRangeException>("factor", () => { new Range(-3, 5).GetScaled(-1); }).Message);
        }

        [Fact]
        public void Contains()
        {
            var sut = new Range(-2, 4);

            Assert.False(sut.Contains(-3));
            Assert.True(sut.Contains(-2));
            Assert.True(sut.Contains(-1));

            Assert.True(sut.Contains(3));
            Assert.True(sut.Contains(4));
            Assert.False(sut.Contains(5));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("min", () => { new Range(null, 1); });
            Assert.Throws<ArgumentNullException>("max", () => { new Range(1, null); });
            Assert.Equal("min is greater than max.", Assert.Throws<ArgumentException>(() => { new Range(1, -1); }).Message);

            Assert.Throws<ArgumentNullException>("value", () => { new Range(-1, 1).Contains(null); });
            Assert.Throws<ArgumentNullException>("thickness", () => { new Range(-1, 1).GetPadded(null); });
            Assert.Throws<ArgumentNullException>("factor", () => { new Range(-1, 1).GetScaled(null); });
        }
    }
}
