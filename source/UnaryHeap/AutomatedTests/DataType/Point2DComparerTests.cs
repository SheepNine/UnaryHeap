using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UnaryHeap.DataType.Tests
{
    public class Point2DComparerTests
    {
        [Test, Sequential]
        public void TestComparison(
            [ValueSource("SortFirstByX")] bool sortFirstByX,
            [ValueSource("SortXDescending")] bool sortXDescending,
            [ValueSource("SortYDescending")] bool sortYDescending,
            [ValueSource("ExpectedSortOrder")] Point2D[] expected)
        {
            var inputData = SortInputData;
            Array.Sort(inputData,
                new Point2DComparer(sortFirstByX, sortXDescending, sortYDescending));
            Assert.AreEqual(expected, inputData);
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

        public static IEnumerable<bool> SortFirstByX
        {
            get
            {
                return new[] {
                    false,
                    false,
                    false,
                    false,
                    true,
                    true,
                    true,
                    true,
                };
            }
        }

        public static IEnumerable<bool> SortXDescending
        {
            get
            {
                return new[] {
                    false,
                    false,
                    true,
                    true,
                    false,
                    false,
                    true,
                    true,
                };
            }
        }

        public static IEnumerable<bool> SortYDescending
        {
            get
            {
                return new[] {
                    false,
                    true,
                    false,
                    true,
                    false,
                    true,
                    false,
                    true,
                };
            }
        }

        public static IEnumerable<Point2D[]> ExpectedSortOrder
        {
            get
            {
                var a = new Point2D(-1, -1);
                var b = new Point2D(01, -1);
                var c = new Point2D(-1, 00);
                var d = new Point2D(01, 00);
                var e = new Point2D(-1, 01);
                var f = new Point2D(01, 01);

                return new[] {
                    new Point2D[] { null, null, a, b, c, d, e, f },
                    new Point2D[] { null, null, e, f, c, d, a, b },
                    new Point2D[] { null, null, b, a, d, c, f, e },
                    new Point2D[] { null, null, f, e, d, c, b, a },
                    new Point2D[] { null, null, a, c, e, b, d, f },
                    new Point2D[] { null, null, e, c, a, f, d, b },
                    new Point2D[] { null, null, b, d, f, a, c, e },
                    new Point2D[] { null, null, f, d, b, e, c, a },
                };
            }
        }

        [Test]
        public void DefaultConfiguration()
        {
            var a = SortInputData;
            var b = SortInputData;

            Array.Sort(a, new Point2DComparer(false, false, false));
            Array.Sort(b, new Point2DComparer());

            Assert.AreEqual(a, b);
        }
    }
}
