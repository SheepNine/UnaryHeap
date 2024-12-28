using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace UnaryHeap.Algorithms.Tests
{
    [TestFixture]
    public class BinarySearchLinkedListTests
    {
        [Test]
        public void BinarySearch()
        {
            var sut = new BinarySearchLinkedList<double>(new[] { 0.0, 1.0, 2.0, 3.0 });
            Assert.AreEqual(4, sut.Length);

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

        [Test]
        public void Remove()
        {
            var digits = Enumerable.Range(-25, 51).Where(d => d != 0.0)
                .Select(d => (double)d).ToList();
            var searchDigits = MakeShuffledCopy(digits, 19870608);

            var sut = new BinarySearchLinkedList<double>(
                Enumerable.Range(-25, 51).Select(d => (double)d));
            Assert.AreEqual(51, sut.Length);

            foreach (var searchDigit in searchDigits)
            {
                var searchResult = sut.BinarySearch(searchDigit, MidpointComparer);
                Assert.AreEqual(1, searchResult.Length);
                Assert.AreEqual(searchDigit, searchResult[0].Data);

                sut.Delete(searchResult[0]);
            }

            Assert.AreEqual(1, sut.Length);
        }

        [Test]
        public void AddAndRemove()
        {
            var digits = Enumerable.Range(-25, 51).Where(d => d != 0.0)
                .Select(d => (double)d).ToList();
            var insertDigits = MakeShuffledCopy(digits, 19830630);
            var searchDigits = MakeShuffledCopy(digits, 19870608);

            var sut = new BinarySearchLinkedList<double>(new[] { 0.0 });
            Assert.AreEqual(1, sut.Length);

            foreach (var insertDigit in insertDigits)
            {
                var searchResult = sut.BinarySearch(insertDigit, PredecessorComparer);

                if (insertDigit < searchResult[0].Data)
                    sut.InsertBefore(insertDigit, searchResult[0]);
                else
                    sut.InsertAfter(searchResult[0], insertDigit);
            }

            Assert.AreEqual(51, sut.Length);

            foreach (var searchDigit in searchDigits)
            {
                var searchResult = sut.BinarySearch(searchDigit, MidpointComparer);
                Assert.AreEqual(1, searchResult.Length);
                Assert.AreEqual(searchDigit, searchResult[0].Data);

                sut.Delete(searchResult[0]);
            }

            Assert.AreEqual(1, sut.Length);
        }

        [Test]
        public void NodesCannotBeShared()
        {
            var sut1 = new BinarySearchLinkedList<double>(new[] { 0.0 });
            var sut2 = new BinarySearchLinkedList<double>(new[] { 0.0 });

            var node = sut1.BinarySearch(0.0, PredecessorComparer)[0];

            Assert.IsTrue(
                Assert.Throws<ArgumentException>(
                () => { sut2.InsertAfter(node, 1.1); }).Message.StartsWith(
                    "Node does not belong to this BinarySearchLinkedList"));

            Assert.IsTrue(
                Assert.Throws<ArgumentException>(
                () => { sut2.InsertBefore(1.1, node); }).Message.StartsWith(
                    "Node does not belong to this BinarySearchLinkedList"));

            Assert.IsTrue(
                Assert.Throws<ArgumentException>(
                () => { sut2.Delete(node); }).Message.StartsWith(
                    "Node does not belong to this BinarySearchLinkedList"));
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { new BinarySearchLinkedList<int>(null); });
            Assert.Throws<ArgumentException>(
                () => { new BinarySearchLinkedList<int>(Enumerable.Empty<int>()); });

            var sut = new BinarySearchLinkedList<int>(new[] { 1 });

            Assert.Throws<ArgumentNullException>(
                () => { sut.BinarySearch(1, null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.BinarySearch<string>("1", null); });


            Assert.Throws<ArgumentNullException>(
                () => { sut.InsertBefore(1, null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.InsertAfter(null, 1); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.Delete(null); });
        }

        static void AssertSingleResult(IBsllNode<double>[] searchResult, double expected)
        {
            Assert.AreEqual(1, searchResult.Length);
            Assert.AreEqual(expected, searchResult[0].Data);
        }

        static void AssertDoubleResult(
            IBsllNode<double>[] searchResult, double expectedPred, double expectedSucc)
        {
            Assert.AreEqual(2, searchResult.Length);
            Assert.AreEqual(expectedPred, searchResult[0].Data);
            Assert.AreEqual(expectedSucc, searchResult[1].Data);
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
