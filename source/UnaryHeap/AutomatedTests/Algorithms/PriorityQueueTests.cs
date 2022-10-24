using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnaryHeap.Algorithms.Tests
{
    [TestFixture]
    public class PriorityQueueTests
    {
        [Test]
        public void FiveElementPermutations(
            [ValueSource("FiveElementPermutationsData")] IEnumerable<char> data)
        {
            var axis = new PriorityQueue<char>(data);
            Assert.AreEqual('A', axis.Peek());
            Assert.AreEqual("ABCDE", MakeStringOfContents(axis));
        }

        [Test]
        public void FiveElementPermutationsManualInsertion(
            [ValueSource("FiveElementPermutationsData")] IEnumerable<char> data)
        {
            var axis = new PriorityQueue<char>();

            foreach (var datum in data)
                axis.Enqueue(datum);

            Assert.AreEqual('A', axis.Peek());
            Assert.AreEqual('A', axis.Dequeue());
            Assert.AreEqual('B', axis.Peek());
            Assert.AreEqual('B', axis.Dequeue());
            Assert.AreEqual('C', axis.Peek());
            Assert.AreEqual('C', axis.Dequeue());
            Assert.AreEqual('D', axis.Peek());
            Assert.AreEqual('D', axis.Dequeue());
            Assert.AreEqual('E', axis.Peek());
            Assert.AreEqual('E', axis.Dequeue());
        }

        [Test]
        public void QuickBrownFox()
        {
            var axis = new PriorityQueue<char>("The Quick Brown Fox Jumps Over The Lazy Dog.");
            Assert.AreEqual("        .BDFJLOQTTaceeeghhikmnoooprrsuuvwxyz",
                MakeStringOfContents(axis));
        }

        public static IEnumerable<IEnumerable<char>> FiveElementPermutationsData
        {
            get
            {
                return new[] {
                    "ABCDE", "BACDE", "ACBDE", "BCADE", "CABDE", "CBADE",
                    "ABDCE", "BADCE", "ACDBE", "BCDAE", "CADBE", "CBDAE",
                    "ADBCE", "BDACE", "ADCBE", "BDCAE", "CDABE", "CDBAE",
                    "DABCE", "DBACE", "DACBE", "DBCAE", "DCABE", "DCBAE",
                    "ABCED", "BACED", "ACBED", "BCAED", "CABED", "CBAED",
                    "ABDEC", "BADEC", "ACDEB", "BCDEA", "CADEB", "CBDEA",
                    "ADBEC", "BDAEC", "ADCEB", "BDCEA", "CDAEB", "CDBEA",
                    "DABEC", "DBAEC", "DACEB", "DBCEA", "DCAEB", "DCBEA",
                    "ABECD", "BAECD", "ACEBD", "BCEAD", "CAEBD", "CBEAD",
                    "ABEDC", "BAEDC", "ACEDB", "BCEDA", "CAEDB", "CBEDA",
                    "ADEBC", "BDEAC", "ADECB", "BDECA", "CDEAB", "CDEBA",
                    "DAEBC", "DBEAC", "DAECB", "DBECA", "DCEAB", "DCEBA",
                    "AEBCD", "BEACD", "AECBD", "BECAD", "CEABD", "CEBAD",
                    "AEBDC", "BEADC", "AECDB", "BECDA", "CEADB", "CEBDA",
                    "AEDBC", "BEDAC", "AEDCB", "BEDCA", "CEDAB", "CEDBA",
                    "DEABC", "DEBAC", "DEACB", "DEBCA", "DECAB", "DECBA",
                    "EABCD", "EBACD", "EACBD", "EBCAD", "ECABD", "ECBAD",
                    "EABDC", "EBADC", "EACDB", "EBCDA", "ECADB", "ECBDA",
                    "EADBC", "EBDAC", "EADCB", "EBDCA", "ECDAB", "ECDBA",
                    "EDABC", "EDBAC", "EDACB", "EDBCA", "EDCAB", "EDCBA",
                };
            }
        }

        static string MakeStringOfContents(PriorityQueue<char> axis)
        {
            var result = new StringBuilder();

            while (false == axis.IsEmpty)
                result.Append(axis.Dequeue());

            return result.ToString();
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            var sut = new PriorityQueue<string>();

            Assert.Throws<InvalidOperationException>(() => { sut.Peek(); });
            Assert.Throws<InvalidOperationException>(() => { sut.Dequeue(); });

            Assert.Throws<ArgumentNullException>(
                () => { new PriorityQueue<string>((IEnumerable<string>)null); });
            Assert.Throws<ArgumentNullException>(
                () => { new PriorityQueue<string>((IComparer<string>)null); });
            Assert.Throws<ArgumentNullException>(
                () => { new PriorityQueue<string>(null, Comparer<string>.Default); });
            Assert.Throws<ArgumentNullException>(
                () => { new PriorityQueue<string>(new[] { "lol" }, null); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { new PriorityQueue<string>(-1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { new PriorityQueue<string>(-1, Comparer<string>.Default); });
            Assert.Throws<ArgumentNullException>(
                () => { new PriorityQueue<string>(0, null); });
        }
    }
}
