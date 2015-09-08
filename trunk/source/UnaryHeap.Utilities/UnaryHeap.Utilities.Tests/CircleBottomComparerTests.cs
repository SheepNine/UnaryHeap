using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class CircleBottomComparerTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SortOrderCorrect()
        {
            var sut = new CircleBottomComparer();
            var circles = new List<Circle2D>();

            foreach (var y in Enumerable.Range(-5, 11))
                foreach (var q in Enumerable.Range(0, 5))
                    circles.Add(new Circle2D(new Point2D(y, y), q));

            Assert.Equal(0, sut.Compare(null, null));

            foreach (var ci in Enumerable.Range(0, circles.Count).Select(i => circles[i]))
            {
                Assert.Equal(-1, sut.Compare(null, ci));
                Assert.Equal(1, sut.Compare(ci, null));

                foreach (var cj in Enumerable.Range(0, circles.Count).Select(j => circles[j]))
                {
                    var bottomComp = Bottom(cj).CompareTo(Bottom(ci)); // Descending order for bottom
                    var centerXComp = ci.Center.X.CompareTo(cj.Center.X); // Ascending order for center X

                    // Bottom ordering has precedence
                    var expected = (bottomComp == 0) ? centerXComp : bottomComp;

                    Assert.Equal(expected, sut.Compare(ci, cj));
                    Assert.Equal(expected, CircleBottomComparer.CompareCircles(ci, cj));
                }
            }
        }

        double Bottom(Circle2D c)
        {
            return (double)(c.Center.Y) - Math.Sqrt((double)c.Quadrance);
        }
    }
}
