using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnaryHeap.Utilities.Misc;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class PriorityQueueTests
    {
        [Theory]
        [MemberData("FiveElementPermutationsData")]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void FiveElementPermutations(IEnumerable<char> data)
        {
            var axis = new PriorityQueue<char>(data);
            Assert.Equal('A', axis.Peek());
            Assert.Equal("ABCDE", MakeStringOfContents(axis));
        }

        [Theory]
        [MemberData("FiveElementPermutationsData")]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void FiveElementPermutationsManualInsertion(IEnumerable<char> data)
        {
            var axis = new PriorityQueue<char>();

            foreach (var datum in data)
                axis.Enqueue(datum);

            Assert.Equal('A', axis.Peek());
            Assert.Equal('A', axis.Dequeue());
            Assert.Equal('B', axis.Peek());
            Assert.Equal('B', axis.Dequeue());
            Assert.Equal('C', axis.Peek());
            Assert.Equal('C', axis.Dequeue());
            Assert.Equal('D', axis.Peek());
            Assert.Equal('D', axis.Dequeue());
            Assert.Equal('E', axis.Peek());
            Assert.Equal('E', axis.Dequeue());
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void QuickBrownFox()
        {
            var axis = new PriorityQueue<char>("The Quick Brown Fox Jumps Over The Lazy Dog.");
            Assert.Equal("        .BDFJLOQTTaceeeghhikmnoooprrsuuvwxyz",
                MakeStringOfContents(axis));
        }

        public static IEnumerable<object[]> FiveElementPermutationsData
        {
            get
            {
                var permutations = new[] {
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

                return permutations.Select(i => new[] { i });
            }
        }

        static string MakeStringOfContents(PriorityQueue<char> axis)
        {
            var result = new StringBuilder();

            while (false == axis.IsEmpty)
                result.Append(axis.Dequeue());

            return result.ToString();
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            var sut = new PriorityQueue<string>();

            Assert.Throws<InvalidOperationException>(() => { sut.Peek(); });
            Assert.Throws<InvalidOperationException>(() => { sut.Dequeue(); });

            Assert.Throws<ArgumentNullException>("collection",
                () => { new PriorityQueue<string>((IEnumerable<string>)null); });
            Assert.Throws<ArgumentNullException>("comparer",
                () => { new PriorityQueue<string>((IComparer<string>)null); });
            Assert.Throws<ArgumentNullException>("collection",
                () => { new PriorityQueue<string>(null, Comparer<string>.Default); });
            Assert.Throws<ArgumentNullException>("comparer",
                () => { new PriorityQueue<string>(new[] { "lol" }, null); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { new PriorityQueue<string>(-1); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { new PriorityQueue<string>(-1, Comparer<string>.Default); });
            Assert.Throws<ArgumentNullException>("comparer",
                () => { new PriorityQueue<string>(0, null); });
        }
    }
}
