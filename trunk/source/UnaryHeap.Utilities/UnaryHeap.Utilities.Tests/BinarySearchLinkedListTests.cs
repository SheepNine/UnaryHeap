using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Utilities.Misc;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class BinarySearchLinkedListTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void BinarySearch()
        {
            var sut = new BinarySearchLinkedList<double>(new[] { 0.0, 1.0, 2.0, 3.0 });
            Assert.Equal(4, sut.Length);

            AssertSingleResult(sut.BinarySearch(0.0, MidpointComparer), 0.0);
            AssertSingleResult(sut.BinarySearch(0.1, MidpointComparer), 0.0);

            AssertSingleResult(sut.BinarySearch(0.9, MidpointComparer), 1.0);
            AssertSingleResult(sut.BinarySearch(1.0, MidpointComparer), 1.0);
            AssertSingleResult(sut.BinarySearch(1.1, MidpointComparer), 1.0);

            AssertSingleResult(sut.BinarySearch(1.9, MidpointComparer), 2.0);
            AssertSingleResult(sut.BinarySearch(2.0, MidpointComparer), 2.0);
            AssertSingleResult(sut.BinarySearch(2.1, MidpointComparer), 2.0);

            AssertSingleResult(sut.BinarySearch(2.9, MidpointComparer), 3.0);
            AssertSingleResult(sut.BinarySearch(3.0, MidpointComparer), 3.0);

            AssertDoubleResult(sut.BinarySearch(0.5, MidpointComparer), 0.0, 1.0);
            AssertDoubleResult(sut.BinarySearch(1.5, MidpointComparer), 1.0, 2.0);
            AssertDoubleResult(sut.BinarySearch(2.5, MidpointComparer), 2.0, 3.0);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Remove()
        {
            var digits = Enumerable.Range(-25, 51).Where(d => d != 0.0)
                .Select(d => (double)d).ToList();
            var searchDigits = MakeShuffledCopy(digits, 19870608);

            var sut = new BinarySearchLinkedList<double>(
                Enumerable.Range(-25, 51).Select(d => (double)d));
            Assert.Equal(51, sut.Length);

            foreach (var searchDigit in searchDigits)
            {
                var searchResult = sut.BinarySearch(searchDigit, MidpointComparer);
                Assert.Equal(1, searchResult.Length);
                Assert.Equal(searchDigit, searchResult[0].Data);

                sut.Delete(searchResult[0]);
            }

            Assert.Equal(1, sut.Length);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void AddAndRemove()
        {
            var digits = Enumerable.Range(-25, 51).Where(d => d != 0.0)
                .Select(d => (double)d).ToList();
            var insertDigits = MakeShuffledCopy(digits, 19830630);
            var searchDigits = MakeShuffledCopy(digits, 19870608);

            var sut = new BinarySearchLinkedList<double>(new[] { 0.0 });
            Assert.Equal(1, sut.Length);

            foreach (var insertDigit in insertDigits)
            {
                var searchResult = sut.BinarySearch(insertDigit, PredecessorComparer);

                if (insertDigit < searchResult[0].Data)
                    sut.InsertBefore(insertDigit, searchResult[0]);
                else
                    sut.InsertAfter(searchResult[0], insertDigit);
            }

            Assert.Equal(51, sut.Length);

            foreach (var searchDigit in searchDigits)
            {
                var searchResult = sut.BinarySearch(searchDigit, MidpointComparer);
                Assert.Equal(1, searchResult.Length);
                Assert.Equal(searchDigit, searchResult[0].Data);

                sut.Delete(searchResult[0]);
            }

            Assert.Equal(1, sut.Length);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void NodesCannotBeShared()
        {
            var sut1 = new BinarySearchLinkedList<double>(new[] { 0.0 });
            var sut2 = new BinarySearchLinkedList<double>(new[] { 0.0 });

            var node = sut1.BinarySearch(0.0, PredecessorComparer)[0];

            Assert.StartsWith("Node does not belong to this BinarySearchLinkedList",
                Assert.Throws<ArgumentException>("node",
                () => { sut2.InsertAfter(node, 1.1); }).Message);

            Assert.StartsWith("Node does not belong to this BinarySearchLinkedList",
                Assert.Throws<ArgumentException>("node",
                () => { sut2.InsertBefore(1.1, node); }).Message);

            Assert.StartsWith("Node does not belong to this BinarySearchLinkedList",
                Assert.Throws<ArgumentException>("node",
                () => { sut2.Delete(node); }).Message);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("data",
                () => { new BinarySearchLinkedList<int>(null); });
            Assert.Throws<ArgumentException>("data",
                () => { new BinarySearchLinkedList<int>(Enumerable.Empty<int>()); });

            var sut = new BinarySearchLinkedList<int>(new[] { 1 });

            Assert.Throws<ArgumentNullException>("comparator",
                () => { sut.BinarySearch(1, null); });
            Assert.Throws<ArgumentNullException>("comparator",
                () => { sut.BinarySearch<string>("1", null); });


            Assert.Throws<ArgumentNullException>("node",
                () => { sut.InsertBefore(1, null); });
            Assert.Throws<ArgumentNullException>("node",
                () => { sut.InsertAfter(null, 1); });
            Assert.Throws<ArgumentNullException>("node",
                () => { sut.Delete(null); });
        }

        static void AssertSingleResult(IBsllNode<double>[] searchResult, double expected)
        {
            Assert.Equal(1, searchResult.Length);
            Assert.Equal(expected, searchResult[0].Data);
        }

        static void AssertDoubleResult(
            IBsllNode<double>[] searchResult, double expectedPred, double expectedSucc)
        {
            Assert.Equal(2, searchResult.Length);
            Assert.Equal(expectedPred, searchResult[0].Data);
            Assert.Equal(expectedSucc, searchResult[1].Data);
        }

        static int MidpointComparer(double searchValue, double pred, double succ)
        {
            Assert.True(pred < succ);
            return searchValue.CompareTo((pred + succ) / 2.0);
        }

        static int PredecessorComparer(double searchValue, double pred, double succ)
        {
            Assert.True(pred < succ);
            return searchValue.CompareTo(pred);
        }

        static List<T> MakeShuffledCopy<T>(List<T> values, int randomSeed)
        {
            var indices = Enumerable.Range(0, values.Count).ToList();
            var random = new Random(randomSeed);

            var result = new List<T>();

            while (indices.Count > 0)
            {
                var indexToUse = random.Next(indices.Count);
                result.Add(values[indices[indexToUse]]);
                indices.RemoveAt(indexToUse);
            }

            return result;
        }
    }
}
