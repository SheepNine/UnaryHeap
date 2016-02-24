using System;
using System.Collections.Generic;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class Point2DComparerTests
    {
        [Theory]
        [MemberData("SortOrders")]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void TestComparison(
            bool sortFirstByX, bool sortXDescending, bool sortYDescending, Point2D[] expected)
        {
            var inputData = SortInputData;
            Array.Sort(inputData,
                new Point2DComparer(sortFirstByX, sortXDescending, sortYDescending));
            Assert.Equal(expected, inputData);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void DefaultConfiguration()
        {
            var a = SortInputData;
            var b = SortInputData;

            Array.Sort(a, new Point2DComparer(false, false, false));
            Array.Sort(b, new Point2DComparer());

            Assert.Equal(a, b);
        }

        public static Point2D[] SortInputData
        {
            get
            {
                return new[] {
                    new Point2D(-1, -1),
                    new Point2D(01, 00),
                    new Point2D(-1, 01), 
                    null,
                    new Point2D(01, 01),
                    null,
                    new Point2D(-1, 00),
                    new Point2D(01, -1),
                };
            }
        }

        public static IEnumerable<object[]> SortOrders
        {
            get
            {
                var a = new Point2D(-1, -1);
                var b = new Point2D(01, -1);
                var c = new Point2D(-1, 00);
                var d = new Point2D(01, 00);
                var e = new Point2D(-1, 01);
                var f = new Point2D(01, 01);

                return new[]{
                    new object[] { false, false, false, new [] { null, null, a, b, c, d, e, f } },
                    new object[] { false, false,  true, new [] { null, null, e, f, c, d, a, b } },
                    new object[] { false,  true, false, new [] { null, null, b, a, d, c, f, e } },
                    new object[] { false,  true,  true, new [] { null, null, f, e, d, c, b, a } },
                    new object[] {  true, false, false, new [] { null, null, a, c, e, b, d, f } },
                    new object[] {  true, false,  true, new [] { null, null, e, c, a, f, d, b } },
                    new object[] {  true,  true, false, new [] { null, null, b, d, f, a, c, e } },
                    new object[] {  true,  true,  true, new [] { null, null, f, d, b, e, c, a } },
                };
            }
        }
    }
}
