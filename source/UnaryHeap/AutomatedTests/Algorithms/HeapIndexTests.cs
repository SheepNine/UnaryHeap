using NUnit.Framework;
using System.Linq;

namespace UnaryHeap.Algorithms.Tests
{
    [TestFixture]
    public class HeapIndexTests
    {
        [Test]
        public void FrontChildIndex()
        {
            var expected = new[] {
                1,
                3, 5,
                7, 9, 11, 13,
                15, 17, 19, 21, 23, 25, 27, 29
            };

            foreach (var input in Enumerable.Range(0, expected.Length))
                Assert.AreEqual(expected[input], input.FrontChildIndex());
        }

        [Test]
        public void BackChildIndex()
        {
            var expected = new[] {
                2,
                4, 6,
                8, 10, 12, 14,
                16, 18, 20, 22, 24, 26, 28, 30
            };

            foreach (var input in Enumerable.Range(0, expected.Length))
                Assert.AreEqual(expected[input], input.BackChildIndex());
        }

        [Test]
        public void ParentIndex()
        {
            var expected = new[] {
                -1,
                0, 0,
                1, 1, 2, 2,
                3, 3, 4, 4, 5, 5, 6, 6
            };

            foreach (var input in Enumerable.Range(0, expected.Length))
                Assert.AreEqual(expected[input], input.ParentIndex());
        }

        [Test]
        public void Depth()
        {
            var expected = new[] {
                0,
                1, 1,
                2, 2, 2, 2,
                3, 3, 3, 3, 3, 3, 3, 3
            };

            foreach (var input in Enumerable.Range(0, expected.Length))
                Assert.AreEqual(expected[input], input.Depth(), $"Incorrect for input {input}");
        }
    }
}
