using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Utilities.D2;
using NUnit.Framework;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class CircleBottomComparerTests
    {
        [Test]
        public void SortOrderCorrect()
        {
            var sut = new CircleBottomComparer();
            var circles = new List<Circle2D>();

            foreach (var y in Enumerable.Range(-5, 11))
                foreach (var q in Enumerable.Range(0, 5))
                    circles.Add(new Circle2D(new Point2D(y, y), q));

            Assert.AreEqual(0, sut.Compare(null, null));

            foreach (var ci in Enumerable.Range(0, circles.Count).Select(i => circles[i]))
            {
                Assert.AreEqual(-1, sut.Compare(null, ci));
                Assert.AreEqual(1, sut.Compare(ci, null));

                foreach (var cj in Enumerable.Range(0, circles.Count).Select(j => circles[j]))
                {
                    // Descending order for bottom
                    var bottomComp = Bottom(cj).CompareTo(Bottom(ci));
                    // Ascending order for center X
                    var centerXComp = ci.Center.X.CompareTo(cj.Center.X);

                    // Bottom ordering has precedence
                    var expected = (bottomComp == 0) ? centerXComp : bottomComp;

                    Assert.AreEqual(expected, sut.Compare(ci, cj));
                    Assert.AreEqual(expected, CircleBottomComparer.CompareCircles(ci, cj));
                }
            }
        }

        double Bottom(Circle2D c)
        {
            return (double)(c.Center.Y) - Math.Sqrt((double)c.Quadrance);
        }
    }
}
